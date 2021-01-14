using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;

namespace XactTodo.Domain.AggregatesModel.IdentityAggregate
{
    /// <summary>
    /// 身份证明信息
    /// </summary>
    /// <remarks>每成功发放/更新一次令牌则记录一次</remarks>
    public class Identity : BaseEntity, IAggregateRoot
    {
        public const int MaxAccessTokenLength = 32;
        public const int MaxRefreshTokenLength = 32;

        public int UserId { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Required]
        [StringLength(User.MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 昵称
        /// </summary>
        [Required]
        [StringLength(User.MaxDisplayNameLength)]
        public string Nickname { get; set; }

        /// <summary>
        /// 访问令牌
        /// </summary>
        [StringLength(MaxAccessTokenLength)]
        public string AccessToken { get; set; }

        /// <summary>
        /// 刷新令牌
        /// </summary>
        [StringLength(MaxRefreshTokenLength)]
        public string RefreshToken { get; set; }

        /// <summary>
        /// 颁发时间
        /// </summary>
        public DateTime IssueTime { get; internal set; }

        /// <summary>
        /// 令牌有效时长(秒)
        /// </summary>
        public int ExpiresIn { get; set; }

        public bool Invalid { get; set; }

    }

}
