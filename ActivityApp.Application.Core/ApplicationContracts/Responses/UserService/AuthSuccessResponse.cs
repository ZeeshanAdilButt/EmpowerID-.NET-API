using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.UserService
{
    public class AuthSuccessResponse
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}
