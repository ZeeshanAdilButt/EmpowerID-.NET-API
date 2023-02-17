using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.ApplicationContracts.Responses
{
    public interface IErrorResponse
    {
        //Errors that we need to show to the user
        List<string> APIErrors { get; set; }
        //Application specific errors
        public List<string> Errors { get; set; }
        int ErrorID { get; set; }
        Exception OriginalException { get; set; }
        bool IsSuccess { get; set; }
        public string CustomErrorMessage { get; set; }
    }
}
