using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Authentication
{
    public static class BearerExtensions
    {
        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder)
            => builder.AddBearer(BearerDefaults.AuthenticationScheme, _ => { });

        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder, Action<BearerOptions> configureOptions)
            => builder.AddBearer(BearerDefaults.AuthenticationScheme, configureOptions);

        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder, string authenticationScheme, Action<BearerOptions> configureOptions)
            => builder.AddBearer(authenticationScheme, displayName: null, configureOptions: configureOptions);

        public static AuthenticationBuilder AddBearer(this AuthenticationBuilder builder, string authenticationScheme, string displayName, Action<BearerOptions> configureOptions)
        {
            //builder.Services.TryAddEnumerable(ServiceDescriptor.Singleton<IPostConfigureOptions<BearerOptions>, BearerPostConfigureOptions>());
            return builder.AddScheme<BearerOptions, BearerHandler>(authenticationScheme, displayName, configureOptions);
        }
    }
}
