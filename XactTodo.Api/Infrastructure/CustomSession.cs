using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;
using XactTodo.Domain;

namespace XactTodo.Api.Infrastructure
{
    public class CustomSession : ICustomSession
    {
        public CustomSession()
        {
        }

        private static CustomSession instance;
        /// <summary>
        /// 当前会话实例
        /// </summary>
        public static CustomSession Current => instance ?? (instance = new CustomSession());

        private string ReadClaim(string claimType)
        {
            var claimsPrincipal = Thread.CurrentPrincipal as ClaimsPrincipal;
            if (claimsPrincipal == null)
            {
                return null;
            }

            var claimsIdentity = claimsPrincipal.Identity as ClaimsIdentity;
            if (claimsIdentity == null)
            {
                return null;
            }

            var claim = claimsIdentity.Claims.FirstOrDefault(c => c.Type == claimType);
            if (claim == null || string.IsNullOrEmpty(claim.Value))
            {
                return null;
            }
            return claim.Value;
        }

        public void VerifyLoggedin()
        {
            if (!UserId.HasValue)
                throw new Exception("用户未登录");
        }

        public string AccessToken
        {
            get { return ReadClaim(ClaimTypes.Sid); }
        }

        public string UserName
        {
            get { return ReadClaim(ClaimTypes.Name); }
        }

        public int? UserId
        {
            get
            {
                var value = ReadClaim(ClaimTypes.NameIdentifier);
                return value == null ? (int?)null : int.Parse(value);
            }
        }

        public string NickName
        {
            get { return ReadClaim(ClaimTypes.Surname); }
        }

        public string Email
        {
            get
            {
                return ReadClaim(ClaimTypes.Email);
            }
        }

    }

}
