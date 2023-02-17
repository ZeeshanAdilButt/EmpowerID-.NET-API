using ActivityApp.Application.Core.ApplicationContracts.Requests.Employee;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using ActivityApp.Domain.Data;
using AutoMapper;

namespace ActivityApp.Application.AutoMapperProfiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            #region Request DTOs Registration

            #region Example Controller

            CreateMap<CreateEmployeeRequest, Employee>();
            
            CreateMap<Employee, CreateEmployeeRequest>();

            CreateMap<UpdateEmployeeRequest, Employee>();

            CreateMap<Employee, GetEmployeeResponse>();
            CreateMap<Employee, CreateEmployeeResponse>();
            CreateMap<Employee, UpdateEmployeeResponse>();

            #endregion

            #endregion
        }
    }
}
