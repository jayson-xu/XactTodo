using System;
using System.Runtime.Serialization;

namespace XactTodo.Domain.Exceptions
{
    [Serializable]
    public class PasswordInvalidException : Exception
    {
        public PasswordInvalidException()
        {
        }

        public PasswordInvalidException(string message) : base(message)
        {
        }

        public PasswordInvalidException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PasswordInvalidException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}