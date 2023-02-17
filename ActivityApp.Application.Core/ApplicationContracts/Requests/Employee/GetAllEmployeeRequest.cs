using System;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.Example
{
    public class GetAllEmployeeRequest : BaseRequest
    {
        public string searchItem { get; set; }

    }
}
