using ActivityApp.Application.Core.ApplicationContracts.Common;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Employee;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActivityApp.Application.Implementations
{
    public interface IEmployeeService
    {
        Task<GetAllEmployeeResponse> GetAllEmployeeAsync(GetAllEmployeeRequest data);
        Task<GenericResponse> DeleteEmployeeAsync(DeleteEmployeeRequest request);
        Task<CreateEmployeeResponse> CreateAsync(CreateEmployeeRequest data);
        Task<GetEmployeeResponse> GetEmployeeAsync(GetEmployeeRequest request);
        Task<UpdateEmployeeResponse> UpdateAsync(UpdateEmployeeRequest request);
    }
}