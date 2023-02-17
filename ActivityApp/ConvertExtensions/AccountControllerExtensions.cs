using Microsoft.AspNetCore.Identity;
using ActivityApp.Application.Core.ApplicationContracts.Requests.UserService;
using ActivityApp.Domain.Data;

namespace ActivityApp.ConvertExtensions
{
    public static class AccountControllerExtensions
    {

        public static AspNetUsers Convert(this UserRequest req)
        {
            return new AspNetUsers { Email = req.Email };
        }
        public static ResetPasswordRequest Convert(this ResetPasswordRequest dto)
        {
            var response = new ResetPasswordRequest();
            response.Password = dto.Password;
            response.ConfirmPassword = dto.ConfirmPassword;
            response.UserId = dto.UserId;

            return response;
        }
        public static RegistrationRequest Convert(this RegistrationRequest dto)
        {
            var response = new RegistrationRequest();
            response.Password = dto.Password;
            response.ConfirmPassword = dto.ConfirmPassword;
            response.Email = dto.Email;

            return response;
        }
    }
}
