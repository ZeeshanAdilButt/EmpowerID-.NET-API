using System.Threading.Tasks;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Interfaces
{
    public interface IErrorLogRepository
    {
        // gets the recent error log id within 10 minutes of interval
        Task<int?> GetRecentErrorLogId(string userId);
    }
}