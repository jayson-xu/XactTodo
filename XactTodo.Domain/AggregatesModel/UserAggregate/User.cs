using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.SeedWork;
using XactTodo.Domain.Utils;

namespace XactTodo.Domain.AggregatesModel.UserAggregate
{
    public class User : FullAuditedEntity, IAggregateRoot
    {
        public const int MaxUserNameLength = 50;
        private const int MaxPasswordLength = 128;
        public const int MaxDisplayNameLength = 50;
        public const int MaxEmailLength = 100;

        public const string UserNameOfAdministrator = "admin";

        /// <summary>
        /// 账号
        /// </summary>
        ///<remarks>遵循通行做法，不再使用AccountNo作为账号字段名</remarks>
        [Required]
        [StringLength(MaxUserNameLength)]
        public string UserName { get; set; }

        /// <summary>
        /// 哈希后的登录密码
        /// </summary>
        [Newtonsoft.Json.JsonIgnore]
        [StringLength(MaxPasswordLength)]
        public string Password { get; set; }

        [Required]
        [StringLength(MaxDisplayNameLength)]
        public string DisplayName { get; set; }

        [Required]
        [StringLength(MaxEmailLength)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public new int? CreatorUserId { get; set; }

        public bool ValidatePassword(string password)
        {
            return (string.IsNullOrEmpty(this.Password) && string.IsNullOrEmpty(password))
                || Hasher.VerifyHashedPassword(this.Password, password);
        }

        public bool ChangePassword(string password, string newPassword)
        {
            if (!ValidatePassword(password))
                return false;
            this.Password = Hasher.HashPassword(newPassword);
            return true;
        }

        public bool IsAdministrator()
        {
            return this.UserName.Equals(UserNameOfAdministrator, StringComparison.InvariantCultureIgnoreCase);
        }

    }
}
