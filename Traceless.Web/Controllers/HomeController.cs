using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Traceless.Web.Models;
using IdentityModel;
using IdentityModel.Client;
using Newtonsoft.Json.Linq;

namespace Traceless.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]//要求验证才能访问
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task Logout()
        {
            //需要确保同时登出本地应用(MvcClient)的Cookies和OpenId Connect(去Identity Server清除单点登录的Session).
            await HttpContext.SignOutAsync("Cookies");
            await HttpContext.SignOutAsync("oidc");
        }

         /// <summary>
        /// 验证
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> ApiTest()
        {
            //首先通过HttpContext获得access token, 然后在请求的Authorization Header加上Bearer Token.
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var client = new HttpClient();
            client.SetBearerToken(accessToken);
            var content = await client.GetStringAsync("http://traceless.site:50001/api/identity");

            ViewData["Message"] = content;
            return Ok(new { value = content });
        }

        
    }
}
