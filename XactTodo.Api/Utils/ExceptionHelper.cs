using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XactTodo.Api.Utils
{
    /// <summary>
    /// 异常处理辅助类
    /// </summary>
    public static class ExceptionHelper
    {
        /// <summary>
        /// 遍历Exception.InnerException,获取全部异常消息。
        /// </summary>
        /// <param name="exception"></param>
        /// <returns></returns>
        public static string AllMessages(this Exception exception)
        {
            StringBuilder sb = new StringBuilder();
            var ex = exception;
            while (ex != null)
            {
                sb.Append(ex.Message).Append(Environment.NewLine);
                ex = ex.InnerException;
            }
            return sb.ToString();
        }

    }

}
