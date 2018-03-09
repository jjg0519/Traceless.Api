using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Traceless.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();

            //关闭了JWT的Claim 类型映射, 以便允许well-known claims.保证它不会修改任何从Authorization Server返回的Claims.
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
                {
                    //使用Cookie作为验证用户的首选方式
                    options.DefaultScheme = "Cookies";
                    //当用户需要登陆的时候, 将使用的是OpenId Connect Scheme.
                    options.DefaultChallengeScheme = "oidc";
                })
                //添加了可以处理Cookie的处理器(handler).
                .AddCookie("Cookies")
                //让上面的handler来执行OpenId Connect 协议.
                .AddOpenIdConnect("oidc", options =>
                {
                    options.SignInScheme = "Cookies";
                    //信任的Identity Server ( Authorization Server).
                    options.Authority = "http://localhost:50000";
                    options.RequireHttpsMetadata = false;

                    #region implicit授权
                    ////Client的识别标志
                    //options.ClientId = "mvc_implicit";
                    ////要把从Authorization Server的Reponse中返回的token们持久化在cookie中.
                    //options.SaveTokens = true;
                    ////既做Authentication又做Authorization. 也就是说我们既要id_token还要token本身.
                    //options.ResponseType = "id_token token";
                    #endregion
                    #region Hybrid 授权
                    //和implicit差不多, 只不过重定向回来的时候, 获取了一个code, 使用这个code可以换取secret然后获取access token.
                    //Client的识别标志
                    options.ClientId = "mvc_code";
                    //需要在网站(MvcClient)上指定Client Secret. 这个不要泄露出去.
                    options.ClientSecret = "secret";
                    
                    //不需要再获取access token了, 而是code, 这意味着使用的是Authorization Code flow.
                    options.ResponseType = "code id_token";

                    //要把从Authorization Server的Reponse中返回的token们持久化在cookie中.
                    options.SaveTokens = true;
                    //还可以告诉它从UserInfo节点获取用户的Claims.
                    options.GetClaimsFromUserInfoEndpoint = true;
                    //需要指定请求访问的scopes: 包括 TracelessApi和离线访问
                    options.Scope.Add("TracelessApi");
                    options.Scope.Add("offline_access");
                    
                    #endregion
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            //确保每次请求都执行authentication
            app.UseAuthentication();
            app.UseStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
