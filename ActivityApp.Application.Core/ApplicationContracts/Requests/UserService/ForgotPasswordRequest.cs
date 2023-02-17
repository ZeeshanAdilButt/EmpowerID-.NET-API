using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class ForgotPasswordRequest : BaseRequest
    {
        [Required]
        public string Email { get; set; }
    }
}
