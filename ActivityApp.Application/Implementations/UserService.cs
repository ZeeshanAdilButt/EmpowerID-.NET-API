using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ActivityApp.Application.ApplicationContracts.Responses;
using ActivityApp.Application.Common;
using ActivityApp.Application.Core.ApplicationContracts.Requests.UserService;
using ActivityApp.Application.Core.ApplicationContracts.Responses.Account;
using ActivityApp.Application.Core.ApplicationContracts.Responses.UserService;
using ActivityApp.Application.Core.Common;
using ActivityApp.Application.Core.Enum;
using ActivityApp.Application.Core.Extensions;
using ActivityApp.Application.Interfaces;
using ActivityApp.Core.Common;
using ActivityApp.Domain.Data;
using ActivityApp.Infrastructure.SQL.Repostiories.Interfaces;
using ActivityApp.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityApp.Application.Implementations
{
    public class UserService : BaseService, IUserService
    {
        //EmailSender _emailSender = new EmailSender();
        private readonly IConfiguration _config;
        public AspNetUsers user { get; set; }
        public UserService(

            UserManager<AspNetUsers> userManager,
            SignInManager<AspNetUsers> signInManager,
            JwtSettings jwtSettings,
            TokenValidationParameters tokenValidationParameters,
            IRefreshTokensRepository refreshTokensRepository,
            RoleManager<AspNetRoles> roleManager,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor,

            IConfiguration configuration) : base(httpContextAccessor, configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtSettings = jwtSettings;
            _tokenValidationParameters = tokenValidationParameters;
            // discuss with steve
            _refreshTokensRepository = refreshTokensRepository;
            _userRepository = userRepository;
            _roleManager = roleManager;
            string username = _httpContextAccessor.HttpContext.User.Identity.Name;
            user = _userRepository.GetByUserName(username); //TODO: shouldn't be in constructor
            _config = configuration;
        }

        #region Properties and Data Members

        private static RandomNumberGenerator rng = RandomNumberGenerator.Create();
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly SignInManager<AspNetUsers> _signInManager;
        private readonly RoleManager<AspNetRoles> _roleManager;
        private readonly JwtSettings _jwtSettings;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IRefreshTokensRepository _refreshTokensRepository;
        private readonly IUserRepository _userRepository;
        #endregion

        public async Task<SignInResult> PasswordSignInAsync(string userName, string password, bool isPersistent, bool lockoutOnFailure)
        {
            AspNetUsers signedUser = await _userManager.FindByNameAsync(userName);
            return await _signInManager.PasswordSignInAsync(signedUser.Email, password, isPersistent, lockoutOnFailure);
        }

        public async Task<AuthenticationResult> GenerateAuthenticationResultForUserAsync(AspNetUsers user)
        {
            try
            {
                AspNetUsers identityUser = await GetUserbyEmail(user.Email);
                if(!(identityUser.EmailConfirmed) || !(identityUser.PhoneNumberConfirmed))
                {
                    return new AuthenticationResult
                    {
                        Success = false,
                        ErrorMessage = "Either Email or Phone Number is not Confirmed"
                    };
                }
                var userRoles = await GetRolesAsync(identityUser);
                var userDetails = _userRepository.GetByUserName(identityUser.Email);

                //List<AspNetUserClaims> UserClaims = new List<AspNetUserClaims>();
                
                var claims = new ClaimsIdentity(new Claim[]
                             {
                                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                                    new Claim(JwtRegisteredClaimNames.Sub, user.Email.ToString()),
                                    new Claim(ApplicationConstants.UserId, identityUser.Id.ToString()),
                                    new Claim(ApplicationConstants.ClientId, userDetails.ClientId.ToString()),
                             });

                //Adding UserClaims to JWT claims
                foreach (var item in userRoles)
                {
                    claims.AddClaim(new Claim(item, item));
                }

                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);

                #region code smell - don't look
                //var claims = new List<Claim>
                //{
                //    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                //    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                //    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                //    new Claim("id", user.Id)
                //};

                //var userClaims = await _userManager.GetClaimsAsync(user);
                //claims.AddRange(userClaims);

                //var userRoles = await _userManager.GetRolesAsync(user);

                //foreach (var userRole in userRoles)
                //{
                //    claims.Add(new Claim(ClaimTypes.Role, userRole));

                //    var role = await _roleManager.FindByNameAsync(userRole);
                //    if (role == null) continue;
                //    var roleClaims = await _roleManager.GetClaimsAsync(role);

                //    foreach (var roleClaim in roleClaims)
                //    {
                //        if (claims.Contains(roleClaim))
                //            continue;

                //        claims.Add(roleClaim);
                //    }
                //}
                #endregion

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    //Expires = DateTime.UtcNow.Add(_jwtSettings.TokenLifetime),
                    Expires = DateTime.UtcNow.AddDays(300),
                    SigningCredentials =
                        new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var refreshToken = new UserRefreshToken
                {
                    JwtId = token.Id,
                    UserId = identityUser.Id,
                    CreationDate = DateTime.UtcNow,
                    ExpiryDate = DateTime.UtcNow.AddMonths(6)
                };

                await _refreshTokensRepository.AddAsync(refreshToken);

                return new AuthenticationResult
                {
                    Success = true,
                    Token = tokenHandler.WriteToken(token),
                    RefreshToken = refreshToken.Token
                };
            }
            catch (Exception ex)
            {
                int errorMsg = ex.Log(_currentUser);

                var response = new AuthenticationResult();
                response.Success = false;

                response.APIErrors.Add(APIErrorConstants.generalError + errorMsg);

                return response;
            }
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string refreshToken)
        {
            var validatedToken = GetPrincipalFromToken(token);

            if (validatedToken == null)
            {
                return new AuthenticationResult { Errors = new List<string> { "Invalid Token" } };
            }

            var expiryDateUnix =
                long.Parse(validatedToken.Claims.Single(x => x.Type == JwtRegisteredClaimNames.Exp).Value);

            //checking the expiry date
            var expiryDateTimeUtc = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                .AddSeconds(expiryDateUnix);

            if (expiryDateTimeUtc > DateTime.UtcNow)
            {
                return new AuthenticationResult { Errors = new List<string> { "This token hasn't expired yet" } };
            }

            var jti = validatedToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti).Value;

            if (jti == null)
            {
                return new AuthenticationResult
                {
                    Errors = new List<string> { "Invalid Token with expired claims" }
                };
            }

            var storedRefreshToken = await _refreshTokensRepository.GetToken(refreshToken);

            if (storedRefreshToken == null)
            {
                return new AuthenticationResult { Errors = new List<string> { "This refresh token does not exist" } };
            }
            //checking if refresh token has been expired
            if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
            {
                return new AuthenticationResult { Errors = new List<string> { "This refresh token has expired" } };
            }
            //if the refresh token has been Invalidated for security
            if (storedRefreshToken.Invalidated)
            {
                return new AuthenticationResult { Errors = new List<string> { "This refresh token has been invalidated" } };
            }
            //if token has been used
            if (storedRefreshToken.Used)
            {
                return new AuthenticationResult { Errors = new List<string> { "This refresh token has been used" } };
            }
            // if the jwt token in the refrresh doesn't match
            if (storedRefreshToken.JwtId != jti)
            {
                return new AuthenticationResult { Errors = new List<string> { "This refresh token does not match this JWT" } };
            }

            storedRefreshToken.Used = true;

            await _refreshTokensRepository.UpdateAsync(storedRefreshToken);

            var user = await _userManager.FindByIdAsync(validatedToken.Claims.Single(x => x.Type == ApplicationConstants.UserId).Value);
            return await GenerateAuthenticationResultForUserAsync(user);
        }

        public async Task<string> Get(AspNetUsers user, CancellationToken ct)
        {
            return await _userManager.GetUserIdAsync(user);
        }

        public async Task<AspNetUsers> GetbyEmail(string normalizedEmail, CancellationToken ct)
        {
            return await _userManager.FindByEmailAsync(normalizedEmail);
        }
        public async Task<AspNetUsers> GetByIdAsync(string userId)
        {
            return await _userRepository.GetByUserIdAsync(userId);
        }
        public async Task<AspNetUsers> GetUserbyEmail(string normalizedEmail)
        {
            return await _userManager.FindByEmailAsync(normalizedEmail);

        }
        public async Task AddToRoleAsync(AspNetUsers user, string role, CancellationToken ct)
        {
            await _userManager.AddToRoleAsync(user, role);
        }

        public async Task<bool> GetUserRole(AspNetUsers user, string role, CancellationToken ct)
        {
            return await _userManager.IsInRoleAsync(user, role);
        }

        public async Task<string> GetPasswordRestToken(AspNetUsers user)
        {
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            return code;
        }
        public async Task<AspNetUsers> GetbyUserName(string userName, CancellationToken ct)
        {
            return await _userManager.FindByNameAsync(userName);
        }
        public AspNetUsers GetByUserName(string userName)
        {
            return _userRepository.GetByUserName(userName);
        }
        public async Task<CreateUserResponseObject> CreateUser(CreateUserRequest request)
        {
            CreateUserResponseObject response = new CreateUserResponseObject();
            var userExist = await _userManager.FindByEmailAsync(request.Email);
            if (userExist != null)
            {
                response.IsSuccess = false;
                response.Errors.Add("User email exist already!");
                return response;
            }
            var result = await _userManager.CreateAsync(new AspNetUsers
            {
                Email = request.Email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                TwoFactorEnabled = request.TwoFactorEnabled,
            },request.Password);
            if (result.Succeeded)
            {
                userExist = await _userManager.FindByEmailAsync(request.Email);
                var role = await _roleManager.FindByIdAsync(request.RoleId.ToString());
                await _userManager.AddToRoleAsync(userExist, role.Name);
                if (role.Id == Convert.ToString((int)RoleEnum.Admin))
                {
                    await _userManager.AddClaimAsync(userExist, new Claim("isAdmin", "true"));
                }
                else
                {
                    await _userManager.AddClaimAsync(userExist, new Claim("isUser", "true"));
                }
                var userEntity = _userRepository.GetByUserEmail(request.Email);
                userEntity.FirstName = request.FirstName;
                userEntity.LastName = request.LastName;
                userEntity.PasswordResetSentDate = DateTime.UtcNow;
                userEntity.Code = Convert.ToString(Guid.NewGuid());
                //TODO: Need to discuss why this logic is required
               
                
                _userRepository.UpdateUser(userEntity);

                string content = $"UId={userEntity.Id}&guid={userEntity.Code}";
                var callbackUrl = $"{configuration["APIUrl"]}Account/ResetPassword?code={EncryptionHelper.Encrypt(content)}";

                //EmailSender _emailSender = new EmailSender();
                //await _emailSender.SendEmailAsync(
                //    request.Email,
                //    "Reset Password",
                //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
                response.IsSuccess = true;
            }

            return response;
        }
        public async Task<RegistrationResponseObject> Register(RegistrationRequest request)
        {
            RegistrationResponseObject response = new RegistrationResponseObject();

            var user = new AspNetUsers { UserName = request.Email, Email = request.Email,PhoneNumberConfirmed = true };

            var result = await _userManager.CreateAsync(user, request.Password);

            if (result.Succeeded)
            {
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

                var callbackUrl = $"{configuration["APIUrl"]}Account/ConfirmEmail?userId={user.Id}&code={code}";
                //await _emailSender.SendEmailAsync(request.Email, "Confirm your email",
                //    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                await _signInManager.SignInAsync(user, isPersistent: false);
                // return LocalRedirect(returnUrl);

                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }

            return response;
        }
        public async Task<ResetPasswordResponseObject> ResetPassword(ResetPasswordRequest request)
        {
            ResetPasswordResponseObject response = new ResetPasswordResponseObject();
            var userEntity = _userRepository.GetByUserId(request.UserId);
            var user = await _userManager.FindByEmailAsync(userEntity.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                response.IsSuccess = false;
                return response;
            }
            userEntity.PasswordResetSentDate = null;
            userEntity.Code = null;
            _userRepository.UpdateUser(userEntity);
            string resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetToken, request.Password);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
            }
            else
            {
                response.IsSuccess = false;
            }
            return response;
        }
        public async Task<ConfirmEmailResponse> GetRegistrationConfirmation(string userId, string code)
        {
            ConfirmEmailResponse response = new ConfirmEmailResponse();
            if (userId == null || code == null)
            {
                response.IsSuccess = false;
                return response;
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                response.APIErrors.Add($"Unable to load user with ID '{userId}'.");
                response.IsSuccess = false;
                return response;
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                response.IsSuccess = true;
                response.Message = $"Thank you for confirming your email.";
            }
            else
            {
                response.IsSuccess = false;
                response.Message = $"Error confirming your email.";
            }
            return response;
        }
        public async Task<ResetPasswordResponse> GetUserIdForResetPassword(string code)
        {
            ResetPasswordResponse resetPasswordResponse = new ResetPasswordResponse();
            if (code == null)
            {
                resetPasswordResponse.APIErrors.Add($"A code must be supplied for password reset.");
                resetPasswordResponse.IsSuccess = false;
                return resetPasswordResponse;
            }
            else
            {
                string[] queryString = EncryptionHelper.Decrypt(code).Split('&');
                string userId = queryString.Count() > 1 ? queryString[0].Replace("UId=", "") : string.Empty;
                string codeGuid = queryString.Count() > 1 ? queryString[1].Replace("guid=", "") : string.Empty;

                var userEntity = _userRepository.GetByUserId(userId);
                int passwordExpireDays = Convert.ToInt32(configuration["PasswordExpireDays"]);
                if (userEntity.PasswordResetSentDate != null && userEntity.PasswordResetSentDate.Value.AddDays(passwordExpireDays) > DateTime.UtcNow
                       && Convert.ToString(userEntity.Code) == codeGuid)
                {
                    userEntity.PasswordResetSentDate = null;
                    userEntity.Code = null;
                    _userRepository.UpdateUser(userEntity);
                    resetPasswordResponse.IsSuccess = true;
                    resetPasswordResponse.UserId = userId;

                }
                else
                {
                    resetPasswordResponse.APIErrors.Add($"Sorry your password reset link has expired or is no longer valid. If you have submitted multiple requests to change your password, please wait a minute and check your inbox for the most recent email message and that will be the valid link.");
                    resetPasswordResponse.IsSuccess = false;
                    return resetPasswordResponse;
                }
            }
            return resetPasswordResponse;
        }
        public async Task<ForgotPasswordResponse> ValidateUserandSendPasswordResetLink(string email)
        {
            ForgotPasswordResponse forgotPasswordResponse = new ForgotPasswordResponse();

            AspNetUsers aspNetUsers = _userRepository.GetByUserEmail(email);
            aspNetUsers.PasswordResetSentDate = DateTime.UtcNow;
            aspNetUsers.Code = Convert.ToString(Guid.NewGuid());
            _userRepository.UpdateUser(aspNetUsers);

            string content = $"UId={aspNetUsers.Id}&guid={aspNetUsers.Code}";
            //string content = "UId=" + aspNetUsers.Id + "&guid=" + aspNetUsers.Code;
            var callbackUrl = $"{configuration["APIUrl"]}Account/ResetPassword?code={EncryptionHelper.Encrypt(content)}";

            //TODO: send emails
            //await _emailSender.SendEmailAsync(
            //    email,
            //    "Reset Password",
            //    $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");
            forgotPasswordResponse.IsSuccess = true;
            return forgotPasswordResponse;
        }
        public async Task<IList<string>> GetRolesAsync(AspNetUsers user)
        {
            return await _userManager.GetRolesAsync(user);
        }
        public async Task<IList<Claim>> GetClaimsAsync(AspNetUsers user)
        {
            return await _userManager.GetClaimsAsync(user);
        }

        #region Factory Methods

        private ClaimsPrincipal GetPrincipalFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var tokenValidationParameters = _tokenValidationParameters.Clone();
                tokenValidationParameters.ValidateLifetime = false;
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);
                if (!IsJwtWithValidSecurityAlgorithm(validatedToken))
                {
                    return null;
                }

                return principal;
            }
            catch
            {
                return null;
            }
        }

        private bool IsJwtWithValidSecurityAlgorithm(SecurityToken validatedToken)
        {
            return validatedToken is JwtSecurityToken jwtSecurityToken &&
                   jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                       StringComparison.InvariantCultureIgnoreCase);

        }
        public virtual string HashPassword(AspNetUsers user, string password)
        {
            //if (_compatibilityMode == PasswordHasherCompatibilityMode.IdentityV2)
            //{
            return Convert.ToBase64String(HashPasswordV2(password, rng));
            //}
            //else
            //{
            //    return Convert.ToBase64String(HashPasswordV3(password, _rng));
            //}
        }

        private static byte[] HashPasswordV2(string password, RandomNumberGenerator rng)
        {
            const KeyDerivationPrf Pbkdf2Prf = KeyDerivationPrf.HMACSHA1; // default for Rfc2898DeriveBytes
            const int Pbkdf2IterCount = 1000; // default for Rfc2898DeriveBytes
            const int Pbkdf2SubkeyLength = 256 / 8; // 256 bits
            const int SaltSize = 128 / 8; // 128 bits

            // Produce a version 2 text hash.
            byte[] salt = new byte[SaltSize];
            rng.GetBytes(salt);
            byte[] subkey = KeyDerivation.Pbkdf2(password, salt, Pbkdf2Prf, Pbkdf2IterCount, Pbkdf2SubkeyLength);

            var outputBytes = new byte[1 + SaltSize + Pbkdf2SubkeyLength];
            outputBytes[0] = 0x00; // format marker
            Buffer.BlockCopy(salt, 0, outputBytes, 1, SaltSize);
            Buffer.BlockCopy(subkey, 0, outputBytes, 1 + SaltSize, Pbkdf2SubkeyLength);
            return outputBytes;
        }



        #endregion
    }
}
