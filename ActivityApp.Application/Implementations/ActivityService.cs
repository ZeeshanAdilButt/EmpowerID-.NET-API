using AutoMapper;
using ActivityApp.Application.Common;
using ActivityApp.Application.ConvertExtensions;
using ActivityApp.Application.Core.ApplicationContracts.Common;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Employee;
using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityApp.Application.Implementations
{
    public class EmployeeService : BaseService, IEmployeeService
    {
        private readonly IEmployeeRepository _EmployeeRepository;

        public EmployeeService(IEmployeeRepository EmployeeRepository, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(httpContextAccessor, configuration)
        {
            this._EmployeeRepository = EmployeeRepository;
        }

        public async Task<CreateEmployeeResponse> CreateAsync(CreateEmployeeRequest request)
        {
            try
            {
                Employee data = new Employee
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    DateOfBirth = request.DateOfBirth,
                    Department = request.Department,
                };

                await _EmployeeRepository.CreateAsync(data);

                return new CreateEmployeeResponse { Id= data.Id ,IsSuccess = true};
            }
            catch (System.Exception ex)
            {

                return new CreateEmployeeResponse
                {
                    OriginalException = ex,
                    IsSuccess = false

                };
            }

        }
        public async Task<GenericResponse> DeleteEmployeeAsync(DeleteEmployeeRequest request)
        {
            try
            {
                await _EmployeeRepository.DeleteEmployeeAsync(request.Id);

                return new GenericResponse { IsSuccess = true };
            }
            catch (System.Exception ex)
            {

                return new GenericResponse { IsSuccess = false, OriginalException = ex };

            }

        }

        public async Task<GetEmployeeResponse> GetEmployeeAsync(GetEmployeeRequest request)
        {
            try
            {
                var response = await _EmployeeRepository.GetEmployeeAsync(request.Id);

                if (response == null)
                    throw new ApplicationException("Employee with provided ID does not exist");
                
                return response.Convert();
            }
            catch (System.Exception ex)
            {
                return new GetEmployeeResponse { OriginalException = ex };
            }

        }


        public async Task<GetAllEmployeeResponse> GetAllEmployeeAsync(GetAllEmployeeRequest request)
        {
            try
            {
                var response = await _EmployeeRepository.GetAllEmployeeAsync(request.searchItem);


                return new GetAllEmployeeResponse
                {
                    IsSuccess = true,
                    Data = response.Select(Employee => Employee.Convert()).ToList()
                };
            }
            catch (System.Exception ex)
            {

                return new GetAllEmployeeResponse { OriginalException = ex };
            }

        }

        public async Task<UpdateEmployeeResponse> UpdateAsync(UpdateEmployeeRequest request)
        {
            try
            {
                Employee requestentity = request.Convert();

                Employee updatedEntity = await _EmployeeRepository.UpdateAsync(requestentity);

                return updatedEntity.ConvertUpdate();

            }
            catch (System.Exception ex)
            {
                return new UpdateEmployeeResponse
                {
                    OriginalException = ex
                };
            }
        }



        
    }
}
