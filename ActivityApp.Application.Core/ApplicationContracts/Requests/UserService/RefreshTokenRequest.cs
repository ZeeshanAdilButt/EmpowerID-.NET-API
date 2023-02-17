using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }
        [Required]
        public string RefreshToken { get; set; }
    }
}
