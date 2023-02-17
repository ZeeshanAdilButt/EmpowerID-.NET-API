using Microsoft.AspNetCore.Identity;
using ActivityApp.Application.Core.ApplicationContracts.Requests.UserService;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Account;
using ActivityApp.Application.Core.ApplicationContracts.Responses.UserService;
using ActivityApp.Domain.Data;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityApp.Application.Interfaces
{
    public interface IUserService
    {
        Task AddToRoleAsync(AspNetUsers user, string role, CancellationToken ct);
        Task<string> Get(AspNetUsers user, CancellationToken ct);
        Task<AspNetUsers> GetbyEmail(string normalizedEmail, CancellationToken ct);
        Task<AspNetUsers> GetByIdAsync(string userId);

        Task<AspNetUsers> GetbyUserName(string userName, CancellationToken ct);
        Task<IList<Claim>> GetClaimsAsync(AspNetUsers user);
        Task<string> GetPasswordRestToken(AspNetUsers user);
        Task<IList<string>> GetRolesAsync(AspNetUsers user);
        Task<AspNetUsers> GetUserbyEmail(string normalizedEmail);
        Task<bool> GetUserRole(AspNetUsers user, string role, CancellationToken ct);

        Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(AspNetUsers user);
        string HashPassword(AspNetUsers user, string password);
        Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure);

        Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken);
        AspNetUsers GetByUserName(string userName);
        Task<ForgotPasswordResponse> ValidateUserandSendPasswordResetLink(string email);
        Task<ResetPasswordResponse> GetUserIdForResetPassword(string code);
        Task<ResetPasswordResponseObject> ResetPassword(ResetPasswordRequest request);
        Task<RegistrationResponseObject> Register(RegistrationRequest request);
        Task<ConfirmEmailResponse> GetRegistrationConfirmation(string userId, string code);
        Task<CreateUserResponseObject> CreateUser(CreateUserRequest request);
    }
}