using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Authentication
{
    public class BearerValidatedContext : ResultContext<BearerOptions>
    {
        public BearerValidatedContext(HttpContext context, AuthenticationScheme scheme, BearerOptions options)
            : base(context, scheme, options)
        {
            //Microsoft.IdentityModel.Tokens.SecurityToken
        }
    }
}
