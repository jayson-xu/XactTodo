using System;
using System.Runtime.Serialization;

namespace XactTodo.Domain.Exceptions
{
    /// <summary>
    /// 数据已在使用中
    /// </summary>
    [Serializable]
    public class DataOccupiedException : Exception
    {
        public DataOccupiedException()
        {
        }

        public DataOccupiedException(string message) : base(message)
        {
        }

        public DataOccupiedException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataOccupiedException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}