# Traceless.Api
一个WEBAPI+NETCORE2.0+SWAGGER+identityServer4的测试运行在CENTOS 7上的DEMO
仅供个人练手，并不是什么轮子，而是轮子的集合练手，代码注释尽量写得比较全，可以参考学习用。
**代码中跳转URL的地方 http://traceless.site 记得改为http://localhost ，不然跳到我的URL上去出现奇怪现象就很有趣了**

## Traceless.Api(Api站点)
Net core 2.0 WebApi
向外提供接口，使用Swagger，接入了简单的identityServer4保护，API文档首页：项目部署URL/Swagger/

## Traceless.Auth（认证中心-已废弃，代码可参考）
identityServer4 Net Core版本，保护API，已经加入官方QucikStartUi

## Traceless.AuthWithASPIdentity(认证中心II-使用ASPIdentity，Traceless.Auth的升级版)
identityServer4 Net Core版本，保护API，使用MySql数据库替代ASPIdentity原有模板中的SQLServer，不再使用TestUser，适合生产环境。

## Traceless.Web（Web客户端）

~~2018年3月6日 在About页面使用implicit OpenId进行登陆并连接到认证中心，认证完成获取数据并显示在About页面

~~2018年3月8日 完成Hybrid授权申请调用被IdentityServer保护的Api并返回结果到ApiTest页~~

2018年3月9日 去除About页面，增加Secure，整合了原来的About和ApiTest页（参考[IdentityServer4官方文档](https://identityserver4.readthedocs.io/en/release/quickstarts/6_aspnet_identity.html)）

相关NETCORE2.0+CENTOS+MYSQL建站文章：https://traceless.site/index.php/archives/15/
