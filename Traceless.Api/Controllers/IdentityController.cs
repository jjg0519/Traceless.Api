using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Traceless.Api.Controllers
{

    /// <summary>
    /// 身份验证相关
    /// </summary>
    [Route("api/[controller]")]
    public class IdentityController : ControllerBase
    {
        /// <summary>
        /// 获取信息
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public IActionResult Get()
        {
            var username = User.Claims.First(x => x.Type == "email").Value;
            return Ok(username);
            //return new JsonResult(from c in User.Claims select new { c.Type, c.Value});
        }
    }
}
