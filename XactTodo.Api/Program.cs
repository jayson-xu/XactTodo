using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using XactTodo.Api.Extensions;
using XactTodo.Api.Infrastructure;
using XactTodo.Infrastructure;
using NLog.Web;

namespace XactTodo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var i = typeof(Domain.AggregatesModel.TeamAggregate.Member).GetInterface(nameof(Domain.SeedWork.ISoftDelete));
            NLog.LogManager.LoadConfiguration("nlog.config");
            CreateWebHostBuilder(args).Build()
                .MigrateDbContext<TodoContext>((context, services) =>
                {
                    //var env = services.GetService<IHostingEnvironment>();
                    //var settings = services.GetService<IOptions<TodoSettings>>();
                    var logger = services.GetService<ILogger<TodoContextSeed>>();

                    new TodoContextSeed()
                        .SeedAsync(context, services, logger)
                        .Wait();
                })
                .Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>().UseNLog();
    }
}
