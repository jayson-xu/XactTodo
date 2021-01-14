using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XactTodo.Domain;
using XactTodo.Security.Session;

namespace XactTodo.Api.Infrastructure
{
    public class MockSession : IClaimsSession
    {
        public string AccessToken => "";

        public int? UserId => 1;

        public string UserName => "Mock";

        public string NickName => "";

        public string Email => "";

        public void VerifyLoggedin()
        {
        }

    }
}
