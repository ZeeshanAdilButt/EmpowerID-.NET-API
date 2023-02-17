using System.Threading.Tasks;
using ActivityApp.Domain.Data;

namespace ActivityApp.Infrastructure.SQL.Repostiories.Interfaces
{
    public interface IRefreshTokensRepository
    {
        Task AddAsync(UserRefreshToken refreshToken);
        Task UpdateAsync(UserRefreshToken refreshToken);
        Task<UserRefreshToken> GetToken(string refreshToken);
    }
}