using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;

namespace Traceless.AuthWithASPIdentity
{
    public class Config
    {
        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                //我们需要把email添加到access token的数据里面, 这就需要告诉Authorization Server的Api Resource里面要包括User的Scope, 因为这是Identity Scope, 我们想要把它添加到access token里
                new ApiResource("TracelessApi", "Traceless API")
                {
                    
                }
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
                    RedirectUris = { "http://localhost:50002/signin-oidc" },
                    //登出之后重定向的网址
                    PostLogoutRedirectUris = { "http://localhost:50002/signout-callback-oidc" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TracelessApi"
                    },

                    //允许返回Access Token
                    AllowAccessTokensViaBrowser = true
                },
                //Hybrid 访问API
                new Client
                {
                    ClientId = "mvc_code",
                    ClientName = "MVC Code Client",
                    //Hybrid或者HybrdAndClientCredentials, 如果只使用Code Flow的话不行, 因为我们的网站使用Authorization Server来进行Authentication, 我们想获取Access token以便被授权来访问api. 所以这里用HybridFlow.
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    RequireConsent = false,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    RedirectUris = { "http://localhost:50002/signin-oidc" },
                    PostLogoutRedirectUris = { "http://localhost:50002/signout-callback-oidc" },
                    //添加一个新的Email scope, 因为我想改变api来允许我基于email来创建用户的数据, 因为authorization server 和 web api是分开的, 所以用户的数据库也是分开的. Api使用用户名(email)来查询数据库中的数据.
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "TracelessApi"
                    },
                    //需要获取Refresh Token, 这就要求我们的网站必须可以"离线"工作, 这里离线是指用户和网站之间断开了, 并不是指网站离线了.
                    AllowOfflineAccess = true,
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
