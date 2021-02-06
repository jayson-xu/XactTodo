using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;
using XactTodo.Domain.Utils;

namespace XactTodo.Api.DTO
{
    public class UserInput
    {
        /// <summary>
        /// 账号
        /// </summary>
        [Required]
        [StringLength(User.MaxUserNameLength)]
        public string Username { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        [Required]
        [MaxLength(32)]
        public string Password { get; set; }

        [Required]
        [StringLength(User.MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(User.MaxEmailLength)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

    }
}
