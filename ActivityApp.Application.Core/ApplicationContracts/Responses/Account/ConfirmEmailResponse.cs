using ActivityApp.Application.Core.ApplicationContracts.Common;

namespace ActivityApp.Application.Core.ApplicationContracts.Responses.Account
{
    public class ConfirmEmailResponse : GenericResponse
    {
        public string Message { get; set; }
    }
}
