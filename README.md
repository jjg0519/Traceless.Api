# Traceless.Api
一个WEBAPI+NETCORE2.0+SWAGGER+identityServer4的测试运行在CENTOS 7上的DEMO

## Traceless.Api(Api站点)
Net core 2.0 WebApi
向外提供接口，使用Swagger，接入了简单的identityServer4保护，API文档首页：项目部署URL/Swagger/

## Traceless.Auth（认证中心）
identityServer4 Net Core版本，保护API，已经加入官方QucikStartUi

## Traceless.Web
在About页面使用implicit OpenId进行登陆并连接到认证中心，认证完成获取数据并显示在About页面

相关NETCORE2.0+CENTOS+MYSQL建站文章：http://traceless.tech/index.php/archives/15/
