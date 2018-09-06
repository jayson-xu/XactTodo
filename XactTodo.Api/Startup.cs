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
using XactTodo.Domain;
using XactTodo.Domain.AggregatesModel.MatterAggregate;
using XactTodo.Infrastructure;
using XactTodo.Infrastructure.Repositories;

namespace XactTodo.WebApi
{
    public class Startup
    {
        private static readonly string swaggerDocName = "v1";

        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public IConfiguration Configuration { get; }
        public ILogger<Startup> Logger { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var env = services.BuildServiceProvider().GetService<IHostingEnvironment>();
            Logger.LogDebug(env.EnvironmentName);
            if (env.IsDevelopment())
            {
                services.AddSingleton<ICustomSession, MockSession>();
            }
            else
            {
                services.AddSingleton<ICustomSession, CustomSession>();
            }
            var connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddScoped<IMatterRepository, MatterRepository>();
            //services.AddScoped<IMatterQueries, MatterQueries>();
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
                // CorsPolicy 是自訂的 Policy 名稱
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });
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
            app.UseCors("CorsPolicy");

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
