using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class RegistrationRequest
    {
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Email { get; set; }
    }
}
