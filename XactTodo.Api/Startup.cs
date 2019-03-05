using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Swashbuckle.AspNetCore.Swagger;
using XactTodo.Api.Infrastructure;
using XactTodo.Api.Queries;
using XactTodo.Api.Utils;
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.IdentityAggregate;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Domain.AggregatesModel.TeamAggregate;
using XactTodo.Domain.AggregatesModel.UserAggregate;
using XactTodo.Domain.SeedWork;
using XactTodo.Infrastructure;
using XactTodo.Infrastructure.Repositories;

namespace XactTodo.WebApi
{
    public class Startup
    {
        private static readonly string swaggerDocName = "v1";
        private static readonly string corsPolicyName = "CorsPolicy";

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }
        public ILogger<Startup> Logger { get; }

        /// <summary>
        /// 自动注册Domain项目中的所有Service类
        /// </summary>
        private void RegisterServiceTypes(IServiceCollection services)
        {
            var types = typeof(IService).Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Service"));
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                //该类实现了IService接口
                if (interfaces.Any(p => p == typeof(IService)))
                {
                    //如果该类实现了除IService接口外的另一个xxService接口，则将该类登记为此接口的实现类
                    var intf = interfaces.FirstOrDefault(p => p != typeof(IService) && p.Name.EndsWith("Service"));
                    if (intf != null)
                    {
                        services.AddScoped(intf, type);
                    }
                    else //否则，将直接使用该类登记依赖注入
                    {
                        services.AddScoped(type, type);
                    }
                }
            }
        }

        /// <summary>
        /// 自动注册Domain项目中的所有仓储类
        /// </summary>
        private void RegisterRepositoryTypes(IServiceCollection services)
        {
            //services.AddScoped<IRepository<User, int>, UserRepository>();
            //不用像上面那样每个仓储类注册一次，下面的代码会自动搜索所有实现了仓储接口的类并注册
            var types = typeof(TodoContext).Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Repository"));
            foreach (var type in types)
            {
                //if(typeof(IRepository<,>).IsAssignableFrom(type)) //这样判断是不行的，因为未明确泛型接口的参数
                var intfIRepositoryName = typeof(IRepository<,>).Name;
                if(type.GetInterface(intfIRepositoryName)==null)
                    continue;
                /* 下面将遍历当前仓储类实现的所有接口，如果该接口为IRepository<,>本身或其派生接口，则登记依赖注入
                 * 不过，在注入仓储实例时，以不同的接口类型声明的变量将无法使用同一仓储类的实例，不过影响不大，重点是方便！
                 * 例如在某(几)个Controller构造函数中分别声明了IRepository<User,int>和IUserRepository类型的变量，
                 * 会分别创建两个UserRepository类的实例，不过因为两个实例中的DbContext实例还是同一个，所以没什么影响。
                 */
                var intfaces = type.GetInterfaces();
                for (var i=0; i< intfaces.Length; i++)
                {
                    var intface = intfaces[i];
                    //留意：intface==typeof(IRepository<,>) 和 intface.Name==typeof(IRepository<,>).Name 并非等效
                    if (intface.Name!=intfIRepositoryName && intface.GetInterface(intfIRepositoryName)==null)
                        continue;
                    if (intface.IsGenericTypeDefinition)
                    {
                        var argTypes = intface.GetGenericArguments();
                        if (argTypes.Length > 0)
                            intface = intface.MakeGenericType(argTypes);
                    }
                    services.AddScoped(intface, type);
                }
            }
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<SmtpConfig>(Configuration.GetSection("Smtp"));
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();
            Logger.LogDebug(env.EnvironmentName);
            if (env.IsDevelopment())
            {
                services.AddSingleton<ICustomSession, MockSession>();
            }
            else
            {
                services.AddSingleton<ICustomSession, MockSession>(); //暂时先用模拟Session
                //services.AddSingleton<ICustomSession, CustomSession>();
            }
            //services.AddScoped<ITeamRepository, TeamRepository>();
            //services.AddScoped<IMatterRepository, MatterRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IIdentityRepository, IdentityRepository>();
            RegisterServiceTypes(services);
            RegisterRepositoryTypes(services);
            services.Add(new ServiceDescriptor(typeof(ITeamQueries), new TeamQueries(connectionString)));
            services.Add(new ServiceDescriptor(typeof(IMatterQueries), new MatterQueries(connectionString)));

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddDbContext<TodoContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseMySql(Configuration.GetConnectionString("DefaultConnection"));
            });

            //跨域
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName, policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
            //Swagger
            services.AddSwaggerGen(c =>
            {
                c.IgnoreObsoleteActions();
                c.SwaggerDoc(
                    // name: 攸關 SwaggerDocument 的 URL 位置。
                    name: swaggerDocName,
                    // info: 是用於 SwaggerDocument 版本資訊的顯示(內容非必填)。
                    info: new Info
                    {
                        Title = "XactTodo API",
                        Version = "1.0.0",
                        Description = "X清单行动系统API接口\r\n<b>注意：</b>所有返回数据的接口，必须判断HTTP Status Code为200或201才表示有数据返回",
                        //TermsOfService = "None",
                        //Contact = new Contact { Name = "Jayson Xu", Url = "xzc@cohl.com" }
                    }
                );
                var security = new Dictionary<string, IEnumerable<string>> { { "Bearer", new string[] { } } };
                c.AddSecurityRequirement(security);//添加一个必须的全局安全信息，和AddSecurityDefinition方法指定的方案名称要一致，这里是Bearer。
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "权限认证(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",//jwt默认的参数名称
                    In = "header",//jwt默认存放Authorization信息的位置(请求头中)
                    Type = "apiKey"
                });//Authorization的设置
                //应用XML注释文档
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Api.xml");
                c.IncludeXmlComments(filePath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //app.UseAuthentication();

            //跨域
            app.UseCors(corsPolicyName);

            app.UseMvc();
            app.UseMvcWithDefaultRoute();
            app.UseStaticFiles();

            //Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(
                    // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                    url: $"../swagger/{swaggerDocName}/swagger.json", //这里一定要使用相对路径，不然网站发布到子目录时将报告："Not Found /swagger/v1/swagger.json"
                                                                      // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                    name: "RESTful API v1.0.0"
                );
                //c.InjectOnCompleteJavaScript();
            });
        }
    }
}
