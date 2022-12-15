using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.Common.Controllers;
using Umbraco.Extensions;
using System.Linq;

using WowsGlobal.Models;
using Umbraco.Cms.Web.Common.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using WowCMS.Helpers;

namespace WowCMS.Controllers
{
    [ApiController]
    [Route("umbraco/api/[controller]")]
    public class UserController : UmbracoApiController
    {
        private readonly IMemberManager _memberManager;
        private readonly IMemberService _memberService;
        private readonly IMemberSignInManager _loginManager;
        private readonly IConfiguration _config;

        public UserController(IMemberManager memberManager,
            IMemberService memberService,
            IMemberSignInManager loginManager,
            IConfiguration config)
        {
            _memberManager = memberManager;
            _memberService = memberService;
            _loginManager = loginManager;
            _config = config;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]User user)
        {
            try
            {
                var result = await _loginManager.PasswordSignInAsync(user.UserName, user.Password, false, false);
                if (result.Succeeded)
                {
                    user.RoleId = 1; // replace with member group type
                    return Ok(new { token = AuthHelper.GenerateJSONWebToken(user, _config) });
                }
                return Unauthorized();
            }
            catch (Exception ex)
            {

                return BadRequest(ex.Message);
            }
        }
    }
}
