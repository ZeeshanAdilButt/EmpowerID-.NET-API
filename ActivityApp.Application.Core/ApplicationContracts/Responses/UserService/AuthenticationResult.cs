using ActivityApp.Application.Core.ApplicationContracts.Common;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.UserService
{
    public class AuthenticationResult : GenericResponse
    {
        public string Token { get; set; }
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string RefreshToken { get; set; }
    }
}
