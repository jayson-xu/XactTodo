using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XactTodo.Api.DTO;
using XactTodo.Api.Queries;
using XactTodo.Api.Utils;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.Utils;
using XactTodo.Security.Session;

namespace XactTodo.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger logger;
        private readonly IUserRepository userRepository;
        private readonly IClaimsSession session;
        private readonly SmtpConfig smtpConfig;

        public UserController(
            ILogger<UserController> logger,
            IUserRepository userRepository,
            IClaimsSession session,
            IOptions<SmtpConfig> smtpConfigAccessor)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.session = session;
            this.smtpConfig = smtpConfigAccessor.Value;
        }

        [HttpPost]
        [Authorize]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.Created)]
        public async Task<IActionResult> Create(UserInput input)
        {
            var user = new Domain.AggregatesModel.UserAggregate.User
            {
                UserName = input.Username,
                Password = Hasher.HashPassword(input.Password),
                DisplayName = input.DisplayName,
                Email = input.Email,
            };
            userRepository.Add(user);
            await userRepository.UnitOfWork.SaveChangesAsync();
            return CreatedAtAction(nameof(Get), new { id = user.Id }, user.Id);
        }

        [HttpGet("{id:int}")] //加上类型声明的好处是，如果传入的参数不是整数则直接返回404，不加则返回400并报告错误"The value 'xxx' is not valid."
        [Authorize]
        [ProducesResponseType(typeof(Queries.User), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                var user = userRepository.GetAll().Where(p => p.Id == id)
                    .Select(p => new Queries.User
                    {
                        Username = p.UserName,
                        DisplayName = p.DisplayName,
                        Email = p.Email,
                        EmailConfirmed = p.EmailConfirmed,
                        CreatorUserId = p.CreatorUserId,
                        //CreatorName = 
                        CreationTime = p.CreationTime,
                    }).FirstOrDefault();

                return Ok(user);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<IActionResult> SendVerificationCode(SendVerificationCodeInput input)
        {
            var user = userRepository.GetAll().FirstOrDefault(p => p.UserName == input.UserName && (!p.EmailConfirmed || p.Email == input.Email));
            if (user == null)
            {
                return BadRequest("未找到与指定用户名及电邮地址相符的账号");
            }
            user.Email = input.Email;
            userRepository.Update(user);
            try
            {
                await SendVerificationCode(user);
            }
            catch(Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "邮件发送失败！" + ex.Message);
            }
            await userRepository.UnitOfWork.SaveChangesAsync();
            return Ok();
        }

        private async Task SendVerificationCode(Domain.AggregatesModel.UserAggregate.User user)
        {
            await EmailHelper.SendMailAsync(
                smtpConfig.Host,
                smtpConfig.EnableSsl,
                smtpConfig.UserName,
                smtpConfig.Password,
                smtpConfig.EmailAddress,
                user.DisplayName,
                user.Email,
                "验证码",
                "您的验证码为："
                );
        }

        public class SendVerificationCodeInput
        {
            public string UserName { get; set; }

            public string Email { get; set; }
        }
    }
}