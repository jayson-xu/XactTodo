using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;
using XactTodo.Api.Utils;

namespace XactTodo.Api.Filters
{
    /// <summary>
    /// 全局异常捕获及处理
    /// </summary>
    public class HttpGlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<HttpGlobalExceptionFilter> log;

        private readonly IWebHostEnvironment env;
        public HttpGlobalExceptionFilter(IWebHostEnvironment env, ILogger<HttpGlobalExceptionFilter> log)
        {
            this.env = env;
            this.log = log;
        }

        /// <summary>
        /// 截获异常
        /// </summary>
        /// <param name="context">异常上下文</param>
        public void OnException(ExceptionContext context)
        {
            try
            {
                var fullMsg = context.Exception.AllMessages();
                var resp = new JsonErrorResponse();
                if ((env as IHostingEnvironment).IsDevelopment())
                {
                    resp.DevelopmentMessage = context.Exception.StackTrace;//堆栈信息
                }
                resp.Message = (env as IHostingEnvironment).IsDevelopment() ? fullMsg : context.Exception.Message;
                // 如果发生用户验证相关的异常
                if (context.Exception is UnauthorizedAccessException || context.Exception.GetType().IsSubclassOf(typeof(UnauthorizedAccessException)))
                {
                    context.Result = new UnauthorizedObjectResult(resp); //未授权
                }
                // 如果是应用程序抛出的异常，则显示错误信息
                else if (context.Exception is ApplicationException || context.Exception.GetType().IsSubclassOf(typeof(ApplicationException)))
                {
                    context.Result = new InternalServerErrorObjectResult(resp);//返回异常数据
                }
                else // 发生预计之外的异常则隐藏具体错误信息
                {
                    resp.Message = (env as IHostingEnvironment).IsDevelopment() ? fullMsg : "发生了未知内部错误";
                    context.Result = new InternalServerErrorObjectResult(resp);
                }

                // 错误日志记录
                log.LogError(context.Exception, context.Exception.GetType().FullName + Environment.NewLine + fullMsg + Environment.NewLine + context.Exception.StackTrace);
            }
            catch (Exception ex)
            {
                log.LogCritical(ex, "糟糕，敌人突破了最后一道防线！！！在全局异常处理类中发生不明异常:(");
            }
        }

        /// <summary>
        /// 表示发生服务器内部异常的结果类
        /// </summary>
        internal class InternalServerErrorObjectResult : ObjectResult
        {
            /// <summary>
            /// 构造函数
            /// </summary>
            /// <param name="value">包含异常内容的对象</param>
            public InternalServerErrorObjectResult(object value) : base(value)
            {
                StatusCode = StatusCodes.Status500InternalServerError;
            }
        }

    }

}