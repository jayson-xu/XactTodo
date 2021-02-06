using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using XactTodo.Domain.SeedWork;
using XactTodo.Domain.Utils;

namespace XactTodo.Api.Queries
{
    public class User
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Username { get; set; }

        public string DisplayName { get; set; }

        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public int? CreatorUserId { get; set; }

        /// <summary>
        /// 创建者名字
        /// </summary>
        public string CreatorName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreationTime { get; set; }
    }
}
