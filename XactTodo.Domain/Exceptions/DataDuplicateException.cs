using System;
using System.Runtime.Serialization;

namespace XactTodo.Domain.Exceptions
{
    [Serializable]
    public class DataDuplicateException : Exception
    {
        public DataDuplicateException()
        {
        }

        public DataDuplicateException(string message) : base(message)
        {
        }

        public DataDuplicateException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected DataDuplicateException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}