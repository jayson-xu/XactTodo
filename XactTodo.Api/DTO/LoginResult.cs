using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Security;

namespace XactTodo.Api.DTO
{
    /// <summary>
    /// 登录结果
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// 结果类型<see cref="LoginResultType"/> 
        /// </summary>
        public LoginResultType ResultType { get; set; }

        /// <summary>
        /// 登录或验证失败时，可通过此属性向前台反馈更明确的错误信息
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        public string Nickname { get; set; }

        /// <summary>
        /// 令牌
        /// </summary>
        public Token Token { get; set; }

        /// <summary>
        /// 无参构造函数
        /// </summary>
        public LoginResult() { }

        /// <summary>
        /// 以指定登录结果类型构造实例
        /// </summary>
        /// <param name="resultType">登录结果类型</param>
        /// <param name="errorMessage">详细错误信息</param>
        public LoginResult(LoginResultType resultType, string errorMessage = null)
        {
            ResultType = resultType;
            ErrorMessage = errorMessage;
        }

        public static LoginResult FromIdentity(Identity identity)
        {
            var result = new LoginResult(LoginResultType.Success)
            {
                UserId = identity.UserId,
                UserName = identity.UserName,
                Nickname = identity.Nickname,
                Token = new Token(identity.AccessToken, identity.RefreshToken, identity.ExpiresIn)
            };
            return result;
        }

    }

    /// <summary>
    /// 登录结果类型
    /// </summary>
    public enum LoginResultType : byte
    {
        /// <summary>
        /// 登录成功
        /// </summary>
        Success = 1,

        /// <summary>
        /// 错误的用户名
        /// </summary>
        InvalidUserName,

        /// <summary>
        /// 密码错误
        /// </summary>
        InvalidPassword,

        /// <summary>
        /// 必须重设密码
        /// </summary>
        PasswordMustReset,

        /// <summary>
        /// 账号未激活
        /// </summary>
        UserIsNotActive,

        /// <summary>
        /// 用户邮件地址未验证，只能使用账号登录
        /// </summary>
        UserEmailIsNotConfirmed,

        /// <summary>
        /// 未知错误
        /// </summary>
        UnkownError = 255,

    }

}
