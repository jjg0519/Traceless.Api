using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Traceless.Auth
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("TracelessApi", "Traceless API")
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client>
            {
                
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    // secret for authentication
                    //ClientSecrets =
                    //{
                    //    new Secret("secret".Sha256())
                    //},

                    // scopes that client has access to
                    AllowedScopes = { "TracelessApi" }
                },
                //implicit获取用户信息的Client
                new Client
                {
                    ClientId = "mvc_implicit",
                    ClientName = "MVC Client",
                    //使用Implicit flow时, 首先会重定向到Authorization Server, 然后登陆, 然后Identity Server需要知道是否可以重定向回到网站, 如果不指定重定向返回的地址的话, 我们的Session有可能就会被劫持. 
                    AllowedGrantTypes = GrantTypes.Implicit,
                    //登陆成功之后重定向的网址
                    RedirectUris = { "http://traceless.site:50002/signin-oidc" },
                    //登出之后重定向的网址
                    PostLogoutRedirectUris = { "http://traceless.site:50002/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TracelessApi"
                    },

                    //允许返回Access Token
                    AllowAccessTokensViaBrowser = true
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<TestUser> Users()
        {
            return new[]
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "admin",
                    Password = "admin"
                }
            };
        }

    }
}
