using System;
using System.Collections.Generic;
using System.Text;

namespace XactTodo.Security.Session
{
    public interface IClaimsSession
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        string AccessToken { get; }

        /// <summary>
        /// 用户Id
        /// </summary>
        int? UserId { get; }

        /// <summary>
        /// 用户名称(账号)
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 昵称
        /// </summary>
        string NickName { get; }

        /// <summary>
        /// 电子邮箱
        /// </summary>
        string Email { get; }

        /// <summary>
        /// 用于确保用户已登录系统，用户未登录系统时将抛出异常供外部业务逻辑捕获并处理
        /// </summary>
        void VerifyLoggedin();

    }
}
