using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Extensions.Logging;
using XactTodo.Domain;
using XactTodo.Infrastructure;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Api.Utils;
using XactTodo.Api.DTO;
using XactTodo.Domain.Exceptions;
using XactTodo.Security;
using XactTodo.Security.Session;

namespace Csci.EasyInventory.WebApi.Controllers
{
    /// <summary>
    /// 用户认证
    /// </summary>
    [Produces("application/json")]
    //[Route("api/auth")]
    [ApiController]
    public class AuthenticationController : Controller
    {
        private readonly IAuthService authService;
        private readonly TodoContext dbContext;
        private readonly ILogger logger;

        public AuthenticationController(
            IAuthService authService,
            TodoContext dbContext,
            //IClaimsSession session, 
            ILogger<AuthenticationController> logger)
        {
            this.dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            this.authService = authService ?? throw new ArgumentNullException(nameof(authService));
            this.logger = logger ?? throw new ArgumentNullException(nameof(dbContext));
        }

        [HttpGet]
        [HttpPost]
        [Route("api/[action]")]
        [AllowAnonymous]
        public IActionResult Hello(string token)
        {
            return Ok("Hello!"+token);
        }

        /// <summary>
        /// 用户登录
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [AllowAnonymous]
        [Route("api/[action]")]
        [ProducesResponseType(typeof(LoginResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Login([FromBody]LoginInput input)
        {
            Identity identity;
            try
            {
                //System.Threading.Thread.Sleep(3000);
                identity = authService.Login(input.UserName, input.Password);
            }
            catch(UserNotFoundException)
            {
                return Ok(new LoginResult(LoginResultType.InvalidUserName, "无效的用户名"));
            }
            catch(PasswordInvalidException)
            {
                return Ok(new LoginResult(LoginResultType.InvalidPassword, "密码错误，请重试！"));
            }
            catch(Exception ex)
            {
                logger.LogError(ex, ex.AllMessages());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            /*if(!identity.OrganizationId.HasValue)
            {
                return new LoginResult(LoginResultType.UnkownError, "用户未分配到任何部门/地盘，请先分配。");
            }*/
            LoginResult result = LoginResult.FromIdentity(identity);
            return Ok(result);
        }

        /// <summary>
        /// 登出
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/[action]")]
        //[Authorize]
        public IActionResult Logout()
        {
            try
            {
                authService.Logout();
            }
            catch(Exception ex)
            {

            }
            return Ok();
        }

        /// <summary>
        /// 请求刷新令牌
        /// </summary>
        /// <param name="token">刷新令牌</param>
        /// <returns></returns>
        [HttpPost]
        [HttpGet]
        [Route("api/[action]")]
        [Authorize]
        [ProducesResponseType(typeof(Token), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Refresh(string token)
        {
            Token tk;
            try
            {
                tk = authService.RefreshToken(token);
            }
            catch(Exception ex)
            {
                //return Forbid();
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            return Ok(tk);
        }

        /// <summary>
        /// 验证令牌是否有效，有效则返回登录信息
        /// </summary>
        /// <param name="token">访问令牌</param>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        [Route("api/[action]")]
        [ProducesResponseType(typeof(LoginResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(string), 500)]
        public IActionResult Validate(string token)
        {
            Identity identity;
            try
            {
                identity = authService.ValidateToken(token);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
            if (identity == null)
            {
                return NotFound();
            }
            var loginResult = LoginResult.FromIdentity(identity);
            return Ok(loginResult);
        }
    }
}