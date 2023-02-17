using System;
using System.Collections.Generic;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class CreateUserRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string RoleId { get; set; }
        public int ClientId { get; set; }
    }
}
