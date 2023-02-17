using ActivityApp.Application.ApplicationContracts.Responses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.ApplicationContracts.Responses
{
    public class ErrorResponse : IErrorResponse
    {
        public ErrorResponse()
        {
            APIErrors = new List<string>();
            Errors = new List<string>();
        }

        public bool IsSuccess { get; set; }
        public List<string> APIErrors { get; set; }
        public List<string> Errors { get; set; }
        public Exception OriginalException { get; set; }
        public string CustomErrorMessage { get; set; }
        public int ErrorID { get; set; }
    }
}
