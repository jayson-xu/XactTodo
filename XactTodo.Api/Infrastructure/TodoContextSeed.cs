using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Infrastructure;

namespace XactTodo.Api.Infrastructure
{
    /// <summary>
    /// EF Core中并没有Seed方法，这里实现相似的功能
    /// </summary>
    public class TodoContextSeed
    {
        /// <summary>
        /// 初始化数据
        /// </summary>

        public async Task SeedAsync(TodoContext context, IServiceProvider services, ILogger<TodoContextSeed> logger)
        {
            var env = services.GetService<IHostingEnvironment>();
            //var context = app.GetRequiredService<MyDbContext>();
            //初始化管理员账号
            if (!context.Users.Any())
            {
                var admin = new User { UserName = "Admin", Password = "", DisplayName = "管理员", Email = "" };
                context.Users.Add(admin);
                await context.SaveChangesAsync();
            }
            //初始化系统设置
            /*if (!context.Settings.Any())
            {
                context.Settings.Add(new Setting { DailyCardQuotaOfLabourer = 5, MonthlyCardQuotaOfLabourer = 20, ValidDaysOfCard = 30 });
                await context.SaveChangesAsync();
            }
            */
        }

    }

}
