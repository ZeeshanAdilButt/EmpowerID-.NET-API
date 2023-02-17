using System;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.Employee
{
    public class DeleteEmployeeRequest : BaseRequest
    {
        public int Id { get; set; }

    }
}
