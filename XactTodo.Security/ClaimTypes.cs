using System;
using System.Collections.Generic;
using System.Text;

namespace XactTodo.Security
{
    /// <summary>
    /// 定义已知的常用声明类型
    /// </summary>
    public class ClaimTypes
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public const string AccessToken = "XactTodo/claims/ACCESSTOKEN";

        /// <summary>
        /// 用户ID
        /// </summary>
        public const string UserId = System.Security.Claims.ClaimTypes.NameIdentifier;

        /// <summary>
        /// 用户名(账号)
        /// </summary>
        public const string UserName = System.Security.Claims.ClaimTypes.Name;

        /// <summary>
        /// 昵称
        /// </summary>
        public const string NickName = "XactTodo/claims/NickName";

        /// <summary>
        /// 手机号码
        /// </summary>
        public const string MobilePhone = System.Security.Claims.ClaimTypes.MobilePhone;

        /// <summary>
        /// 电子邮箱
        /// </summary>
        public const string Email = System.Security.Claims.ClaimTypes.Email;
    }
}
