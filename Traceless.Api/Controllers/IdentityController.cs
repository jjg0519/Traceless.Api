using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Traceless.Api.Controllers
{
    /// <summary>
    /// 身份验证相关
    /// </summary>
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        [Route("api/[controller]/GetUser"),HttpGet]
        public IActionResult Get()
        {
            var username = User.Claims.First(x => x.Type == "email").Value;
            return Ok(username);
            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value});
        }

        /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [Route("api/[controller]/GetIdentity"), HttpGet]
        public async Task<IActionResult> GetIdentity()
        {
            //正式环境中应该在401之后, 调用这个方法, 如果再失败, 再返回错误.
            await RefreshTokensAsync();
            //首先通过HttpContext获得access token, 然后在请求的Authorization Header加上Bearer Token.
            var token = await HttpContext.GetTokenAsync("access_token");
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var content = await client.GetStringAsync("http://traceless.site:50001/api/identity/GetUser");
                // var json = JArray.Parse(content).ToString();
                return Ok(new { value = content });
            }
        }

        /// <summary>
        /// 刷新token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task RefreshTokensAsync()
        {
            //使用一个叫做discovery client的东西来获取Authorization Server的信息. Authorization Server里面有一个discovery节点(endpoint), 可以通过这个地址查看: /.well-known/openid-configuration. 从这里可以获得很多信息, 例如: authorization节点, token节点, 发布者, key, scopes等等.
            var authorizationServerInfo = await DiscoveryClient.GetAsync("http://localhost:50000/");
            //使用TokenClient, 参数有token节点, clientId和secret. 然后可以使用这个client和refreshtoken来请求新的access token等. 
            var client = new TokenClient(authorizationServerInfo.TokenEndpoint, "mvc_code", "secret");
            //refresh token后, 使用client获取新的tokens, 返回结果是tokenresponse. 你可以设断点查看一下token reponse里面都有什么东西, 里面包括identitytoken, accesstoken, refreshtoken等等.
            var refreshToken = await HttpContext.GetTokenAsync("refresh_token");
            var response = await client.RequestRefreshTokenAsync(refreshToken);
           // 需要找到原来的identity token, 因为它相当于是cookie中存储的主键
            var identityToken = await HttpContext.GetTokenAsync("identity_token");
            //设置一下过期时间
            var expiresAt = DateTime.UtcNow + TimeSpan.FromSeconds(response.ExpiresIn);
            //将老的identity token和新获取到的其它tokens以及过期时间, 组成一个集合.
            var tokens = new[]
            {
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.IdToken,
                    Value = identityToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.AccessToken,
                    Value = response.AccessToken
                },
                new AuthenticationToken
                {
                    Name = OpenIdConnectParameterNames.RefreshToken,
                    Value = response.RefreshToken
                },
                new AuthenticationToken
                {
                    Name = "expires_at",
                    Value = expiresAt.ToString("o", CultureInfo.InvariantCulture)
                }
            };
            //使用这些tokens来重新登陆用户. 不过首先要获取当前用户的authentication信息, 使用HttpContext.AuthenticateAsync("Cookies"), 参数是AuthenticationScheme. 然后修改属性, 存储新的tokens.
            var authenticationInfo = await HttpContext.AuthenticateAsync("Cookies");
            authenticationInfo.Properties.StoreTokens(tokens);
            //重登录, 把当前用户信息的Principal和Properties传进去. 这就会更新客户端的Cookies, 用户也就保持登陆并且刷新了tokens.
            await HttpContext.SignInAsync("Cookies", authenticationInfo.Principal, authenticationInfo.Properties);
        }
    }
}
