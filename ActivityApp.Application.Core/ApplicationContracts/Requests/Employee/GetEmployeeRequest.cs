using System;

namespace ActivityApp.Application.Core.ApplicationContracts.Requests.Employee
{
    public class GetEmployeeRequest : BaseRequest
    {
        public int Id { get; set; }

    }
}
