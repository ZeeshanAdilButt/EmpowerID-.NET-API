using Microsoft.EntityFrameworkCore;
using ActivityApp.Infrastructure.SQL;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ActivityApp.Domain.Data;
using System;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Implementations
{
    public class EmployeeRepository : IEmployeeRepository
    {

        ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> CreateAsync(Employee acticvity)
        {
            _context.Employees.Add(acticvity);
            return await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteEmployeeAsync(int Id)
        {
            var entity = _context.Employees.FirstOrDefault(x => x.Id == Id);

            if (entity != null)
            {
                entity.IsActive = false;
                _context.Employees.Update(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            else throw new ApplicationException("Employee with provided ID does not exist");

        }

        public async Task<Employee> GetEmployeeAsync(int Id)
        {
            return await _context.Employees.FirstOrDefaultAsync(x => x.Id == Id && x.IsActive == true);
        }

        public async Task<List<Employee>> GetAllEmployeeAsync(string searchTerm)
        {
            IQueryable<Employee> query = _context.Employees.Where(x => x.IsActive == true);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(x =>
                    x.FirstName.ToLower().Contains(searchTerm) ||
                    x.LastName.ToLower().Contains(searchTerm) ||
                    x.Email.ToLower().Contains(searchTerm) ||
                    x.Department.ToLower().Contains(searchTerm));
            }

            return await query.ToListAsync();
        }



        public async Task<Employee> UpdateAsync(Employee entity)
        {
            var Employee = await _context.Employees.AsNoTracking().FirstOrDefaultAsync(x => x.Id == entity.Id);

            if (Employee == null)
                throw new ApplicationException("Employee with provided ID does not exist");

            _context.Employees.Update(entity);
            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
