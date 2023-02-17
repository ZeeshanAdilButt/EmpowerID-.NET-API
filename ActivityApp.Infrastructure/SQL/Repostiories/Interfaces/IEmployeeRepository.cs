using ActivityApp.Domain.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Interfaces
{
    public interface IEmployeeRepository
    {
        Task<int> CreateAsync(Employee acticvity);
        Task<bool> DeleteEmployeeAsync(int Id);
        Task<Employee> GetEmployeeAsync(int Id);
        Task<List<Employee>> GetAllEmployeeAsync(string searchItem);
        Task<Employee> UpdateAsync(Employee entity);
    }
}