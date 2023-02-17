using System.ComponentModel.DataAnnotations;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.UserService
{
    public class UserRequest : BaseRequest
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}