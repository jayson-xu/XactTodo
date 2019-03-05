using System;
using System.Runtime.Serialization;

namespace XactTodo.Domain.Exceptions
{
    /// <summary>
    /// 数据未找到
    /// </summary>
    [Serializable]
    public class DataNotFoundException : Exception
    {
        public DataNotFoundException()
        {
        }

        public DataNotFoundException(string message) : base(message)
        {
        }

        public DataNotFoundException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}