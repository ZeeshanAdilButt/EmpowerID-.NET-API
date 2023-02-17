using ActivityApp.Domain.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Interfaces
{
    public interface IUserRepository
    {
        void Delete(string userId, int clientId, bool IsSuperClient);
        List<AspNetUsers> GetAllUser();
        AspNetUsers GetByUserEmail(string email);
        Task<AspNetUsers> GetByUserIdAsync(string userId);
        AspNetUsers GetByUserName(string username);
        AspNetUsers UpdateUser(AspNetUsers aspNetUsers);
        AspNetUsers GetByUserId(string userId);
        List<AspNetUsers> GetAllUserByClientId(int clientId);
    }
}