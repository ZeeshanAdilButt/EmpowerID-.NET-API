using ActivityApp.Application.Core.ApplicationContracts.Common;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.Account
{
    public class ResetPasswordResponse : GenericResponse
    {
        public string UserId { get; set; }
    }
}
