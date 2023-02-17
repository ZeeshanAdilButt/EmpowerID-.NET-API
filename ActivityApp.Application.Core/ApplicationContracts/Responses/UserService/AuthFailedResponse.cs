using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.UserService
{
    public class AuthFailedResponse
    {
        public IEnumerable<string> Errors { get; set; }
    }
}
