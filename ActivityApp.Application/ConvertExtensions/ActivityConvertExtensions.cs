using ActivityApp.Application.Core.ApplicationContracts.Requests.Example;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Example;
using ActivityApp.Domain.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityApp.Application.ConvertExtensions
{
    public static class EmployeeConvertExtensions
    {
        public static GetEmployeeResponse Convert(this Employee Employee)
        {
            return new GetEmployeeResponse
            {
                Id = Employee.Id,
                FirstName = Employee.FirstName,
                LastName = Employee.LastName,
                Email = Employee.Email,
                DateOfBirth = Employee.DateOfBirth,
                Department = Employee.Department,

            };
        }

        public static Employee Convert(this UpdateEmployeeRequest request)
        {
            return new Employee
            {
                Id = request.Id,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email   = request.Email,
                DateOfBirth = request.DateOfBirth,
                Department = request.Department,
            };
        }

        public static UpdateEmployeeResponse ConvertUpdate(this Employee updatedEntity)
        {
            return new UpdateEmployeeResponse
            {
                Id = updatedEntity.Id,
                FirstName = updatedEntity.FirstName,
                LastName = updatedEntity.LastName,
                DateOfBirth = updatedEntity.DateOfBirth,
                Department = updatedEntity.Department,



            };
        }

        public static UpdateEmployeeResponse ConvertUpdatedParticipant(this Employee updatedEntity)
        {
            return new UpdateEmployeeResponse
            {
                Id = updatedEntity.Id,
                FirstName = updatedEntity.FirstName,
                LastName = updatedEntity.LastName,
                DateOfBirth = updatedEntity.DateOfBirth,
                Department = updatedEntity.Department,


            };
        }
    }
}
