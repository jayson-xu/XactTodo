using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Utils
{
    public class SmtpConfig
    {
        public string Host { get; set; }
        public bool EnableSsl { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAddress { get; set; }
    }
}
