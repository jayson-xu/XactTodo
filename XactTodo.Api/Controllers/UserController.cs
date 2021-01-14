using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XactTodo.Api.Utils;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.UserAggregate;
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
            IOptions<SmtpConfig> smtpConfig)
        {
            this.logger = logger;
            this.userRepository = userRepository;
            this.session = session;
            this.smtpConfig = smtpConfig.Value;
        }

        [HttpPost]
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

        private async Task SendVerificationCode(User user)
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