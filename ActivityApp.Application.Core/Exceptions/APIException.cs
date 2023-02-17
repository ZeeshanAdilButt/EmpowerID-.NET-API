using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.Exceptions
{
    public class APIException : Exception
    {
        public APIException(string message) : base(message) { }

        public string CustomMessage { get; set; }
        public Exception CustomException { get; set; }
    }
}
