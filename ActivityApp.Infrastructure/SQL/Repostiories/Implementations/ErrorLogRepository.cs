using Microsoft.EntityFrameworkCore;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Implementations
{
    public class ErrorLogRepository : IErrorLogRepository
    {
        private readonly ApplicationDbContext _context;
        public ErrorLogRepository(ApplicationDbContext applicationContext)
        {
            _context = applicationContext;
        }

        public async Task<int?> GetRecentErrorLogId(string userId)
        {
            DateTime dateTime = DateTime.UtcNow.AddMinutes(-10);
            ErrorLog errorLog = await _context.ErrorLog.Where(el => el.CreateDate > dateTime && el.UserId == userId).FirstOrDefaultAsync();
            return errorLog != null ? (int?)errorLog.Id : null;
        }
    }
}
