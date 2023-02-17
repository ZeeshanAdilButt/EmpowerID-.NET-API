using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class ResetPasswordRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserId { get; set; }
    }
}
