using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace TodoTxt.Core
{
    public  class TodoTxtException : Exception
    {
        public TodoTxtException()
        {
        }

        public TodoTxtException(string? message) : base(message)
        {
        }

        public TodoTxtException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected TodoTxtException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
