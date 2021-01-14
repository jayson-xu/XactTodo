using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Authentication
{
    public class BearerOptions : AuthenticationSchemeOptions
    {
        public BearerOptions()
        {
            ClaimsIssuer = "My Self!!!";
        }

        public bool RequireHttpsMetadata { get; internal set; }
    }
}
