using System;
using System.Runtime.Serialization;

namespace XactTodo.Exceptions
{
    [Serializable]
    public class NotLoggedinException : Exception
    {
        public NotLoggedinException()
        {
        }

        public NotLoggedinException(string message) : base(message)
        {
        }

        public NotLoggedinException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NotLoggedinException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}