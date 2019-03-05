using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace XactTodo.Domain.Exceptions
{
    /// <summary>
    /// 代表领域异常的异常类
    /// </summary>
    public class DomainException : Exception
    {
        public DomainException()
        {
        }

        public DomainException(string message) : base(message)
        {
        }

        public DomainException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
