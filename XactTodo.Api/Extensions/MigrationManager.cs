using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace XactTodo.Api.Extensions
{
    public static class MigrationManager
    {
        public static IHost MigrateDatabase<TContext>(this IHost host)
            where TContext : DbContext
        {
            using (var scope = host.Services.CreateScope())
            {
                //var log = scope.ServiceProvider.GetRequiredService<ILogger<Startup>>();
                using (var appContext = scope.ServiceProvider.GetRequiredService<TContext>())
                {
                    try
                    {
                        //appContext.Database.EnsureCreated();
                        //如果写了上面这一句再执行迁移会导致数据库中的__efmigrationshistory表无记录，而重复创建已存在的表
                        //EnsureCreated方法有一段关键的注释：If you are targeting a relational database and using migrations, you can use the DbContext.Database.Migrate() method to ensure the database is created and all migrations are applied.
                        appContext.Database.Migrate();
                    }
                    catch (Exception ex)
                    {
                        //log.LogError(ex.Message);
                        throw;
                    }
                }
            }
            return host;
        }
    }
}
