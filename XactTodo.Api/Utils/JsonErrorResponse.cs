namespace XactTodo.Api.Utils
{
    /// <summary>
    /// 错误响应
    /// </summary>
    public class JsonErrorResponse
    {
        /// <summary>
        /// 错误消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 开发环境的消息
        /// </summary>
        public string DevelopmentMessage { get; set; }

        /// <summary>
        /// 缺省构造函数
        /// </summary>
        public JsonErrorResponse() { }

        /// <summary>
        /// 
        /// </summary>
        public JsonErrorResponse(string message, string developmentMessage = null)
        {
            Message = message;
            DevelopmentMessage = developmentMessage;
        }

    }
}