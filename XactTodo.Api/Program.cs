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
using Microsoft.Extensions.Hosting;

namespace XactTodo.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            Console.WriteLine($"AppContext.BaseDirectory:{AppContext.BaseDirectory}");
            NLog.LogManager.LoadConfiguration("NLog.config"); //注意大小写一致，Linux下文件名是区分大小写的
            CreateHostBuilder(args)
                .Build()
                .MigrateDatabase<TodoContext>()
                .Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.ConfigureKestrel(options => options.Limits.MaxRequestBodySize = 1024 * 1024 * 50);
                    webBuilder
                        .UseStartup<Startup>()
                        .UseNLog(); //使用NLog记录日志
                });
    }
}
