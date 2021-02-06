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
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using NLog;
using Swashbuckle.AspNetCore.Swagger;
using XactTodo.Api.Authentication;
using XactTodo.Api.Filters;
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
using XactTodo.Security;
using XactTodo.Security.Session;

namespace XactTodo.Api
{
    public class Startup
    {
        private static readonly string swaggerDocName = "v1";
        private static readonly string corsPolicyName = "CorsPolicy";
        private readonly IWebHostEnvironment hostEnvironment;
        private readonly Logger logger;

        public Startup(//ILogger<Startup> logger,
            IConfiguration configuration,
            IWebHostEnvironment hostEnvironment)
        {
            Configuration = configuration;
            this.hostEnvironment = hostEnvironment;
            // asp.net core 3.0不能在Startup类构造函数中注入ILogger实例，但是可以在Configure函数通过参数注入
            // 要想在构造函数或ConfigureServices()方法内输出日志，只能直接通过日志框架进行，如下行代码所示
            this.logger = NLog.LogManager.GetLogger(nameof(Startup));
        }

        public IConfiguration Configuration { get; }


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
        /// 自动注册Domain项目中的所有实现IQueries接口的类
        /// </summary>
        private void RegisterQueries(IServiceCollection services)
        {
            var types = typeof(IQueries).Assembly.GetTypes()
                .Where(p => p.IsClass && !p.IsAbstract && p.Name.EndsWith("Queries"));
            foreach (var type in types)
            {
                var interfaces = type.GetInterfaces();
                //该类实现了IQueries接口
                if (interfaces.Any(p => p == typeof(IQueries)))
                {
                    //如果该类实现了除IQueries接口外的另一个更具体的xxQueries接口，则将该类登记为此接口的实现类
                    var intf = interfaces.FirstOrDefault(p => p != typeof(IQueries) && p.Name.EndsWith("Queries"));
                    if (intf != null)
                    {
                        services.AddSingleton(intf, type); //使用单例
                    }
                    else //否则，将直接使用该类登记依赖注入
                    {
                        services.AddSingleton(type, type);
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
                if (type.GetInterface(intfIRepositoryName) == null)
                    continue;
                /* 下面将遍历当前仓储类实现的所有接口，如果该接口为IRepository<,>本身或其派生接口，则登记依赖注入
                 * 不过，在注入仓储实例时，以不同的接口类型声明的变量将无法使用同一仓储类的实例，不过影响不大，重点是方便！
                 * 例如在某(几)个Controller构造函数中分别声明了IRepository<User,int>和IUserRepository类型的变量，
                 * 会分别创建两个UserRepository类的实例，不过因为两个实例中的DbContext实例还是同一个，所以没什么影响。
                 */
                var intfaces = type.GetInterfaces();
                for (var i = 0; i < intfaces.Length; i++)
                {
                    var intface = intfaces[i];
                    //留意：intface==typeof(IRepository<,>) 和 intface.Name==typeof(IRepository<,>).Name 并非等效
                    if (intface.Name != intfIRepositoryName && intface.GetInterface(intfIRepositoryName) == null)
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
            logger.Debug(hostEnvironment.EnvironmentName);
            /*if (hostEnvironment.IsDevelopment())
            {
                services.AddSingleton<IClaimsSession, MockSession>();
            }
            else*/
            {
                //services.AddSingleton<IClaimsSession, MockSession>(); //暂时先用模拟Session
                services.AddSingleton<IClaimsSession, ClaimsSession>();
            }
            //services.AddScoped<ITeamRepository, TeamRepository>();
            //services.AddScoped<IMatterRepository, MatterRepository>();
            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IIdentityRepository, IdentityRepository>();
            RegisterServiceTypes(services);
            RegisterRepositoryTypes(services);
            RegisterQueries(services);
            //添加身份验证
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = BearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = BearerDefaults.AuthenticationScheme;
            }).AddBearer();

            services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<TodoContext>(options =>
            {
                //options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
                options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            });

            // 注入全局异常捕获
            services.AddMvc(o =>
            {
                o.Filters.Add(typeof(HttpGlobalExceptionFilter));
            });

            //添加跨域
            services.AddCors(options =>
            {
                options.AddPolicy(corsPolicyName,
                    builder =>
                    {
                        builder.SetIsOriginAllowed(_ => true)
                            .AllowCredentials()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            //Swagger支持
            bool.TryParse(Configuration.GetSection("SwaggerDoc:Enabled").Value ?? "true", out bool swaggerEnabled);
            if (swaggerEnabled)
            {
                services.AddSwaggerGen(options =>
                {
                    options.IgnoreObsoleteActions();
                    var info = new OpenApiInfo
                    {
                        Version = Configuration.GetSection("SwaggerDoc:Version").Value,
                        Title = Configuration.GetSection("SwaggerDoc:Title").Value,
                        Description = Configuration.GetSection("SwaggerDoc:Description").Value,
                    };
                    //读取appsettings.json文件中的联系人信息
                    string contactName = Configuration.GetSection("SwaggerDoc:ContactName").Value;
                    string contactNameEmail = Configuration.GetSection("SwaggerDoc:ContactEmail").Value;
                    string s = Configuration.GetSection("SwaggerDoc:ContactUrl").Value;
                    Uri contactUrl = string.IsNullOrEmpty(s) ? null : new Uri(s);
                    if (!string.IsNullOrEmpty(contactName))
                        info.Contact = new OpenApiContact { Name = contactName, Email = contactNameEmail, Url = contactUrl };
                    //info.License = new OpenApiLicense { Name = contactName, Url = contactUrl }
                    options.SwaggerDoc(swaggerDocName, info);
                    //添加 Bearer Token 认证
                    options.AddSecurityRequirement(new OpenApiSecurityRequirement{
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference()
                                {
                                    Id = "Bearer",
                                    Type = ReferenceType.SecurityScheme
                                }
                            }, Array.Empty<string>() }
                    });
                    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
                    {
                        Description = "权限认证(数据将在请求头中进行传输) 参数结构: \"Authorization: Bearer {token}\"",
                        Name = "Authorization",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                    });
                    //应用XML注释文档
                    var xmlPath = Path.Combine(hostEnvironment.ContentRootPath, "Api.xml");
                    if (File.Exists(xmlPath))
                        options.IncludeXmlComments(xmlPath);
                });
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app)
        {
            if ((hostEnvironment as IHostEnvironment).IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();
            // 支持跨域(这一行要在app.UseRouting 和 UseEndpoints 之间)
            app.UseCors(corsPolicyName);

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });
            app.UseStaticFiles();

            //Swagger支持
            bool.TryParse(Configuration.GetSection("SwaggerDoc:Enabled").Value ?? "true", out bool swaggerEnabled);
            if (swaggerEnabled)
            {
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint(
                        // url: 需配合 SwaggerDoc 的 name。 "/swagger/{SwaggerDoc name}/swagger.json"
                        url: $"../swagger/{swaggerDocName}/swagger.json", //这里一定要使用相对路径，不然网站发布到子目录时将报告："Not Found /swagger/v1/swagger.json"
                                                                          // description: 用於 Swagger UI 右上角選擇不同版本的 SwaggerDocument 顯示名稱使用。
                        name: "RESTful API v1.0.0"
                    );
                });
            }
        }
    }
}
