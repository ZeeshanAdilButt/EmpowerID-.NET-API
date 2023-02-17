using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ActivityApp.Application.Core.APIController;
using ActivityApp.Application.Core.ApplicationContracts.Requests.UserService;
using ActivityApp.Application.Core.ApplicationContracts.Responses.UserService;
using ActivityApp.Application.Core.Exceptions;
using ActivityApp.Application.Interfaces;
using ActivityApp.ConvertExtensions;
using ActivityApp.Domain.Data;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ActivityApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AccountController : APIBaseController
    {
        private readonly IMapper _mapper;
        public AccountController(
            IUserService userService,
            IMapper mapper,
            IHttpContextAccessor httpContextAccessor,
            IConfiguration configuration,

            ILogger<AccountController> logger) : base(httpContextAccessor, configuration, logger)
        {
            _userService = userService;
            _mapper = mapper;
        }

        #region Properties and Data Memebers
        public const string UserId = "UserId";
        private readonly IUserService _userService;
        #endregion

        /// <summary>
        /// returns token and refresh token depending upon the passed email and password.
        /// API Path:  api/account/login
        /// </summary>
        /// <param name="paramUser">Username and Password</param>
        /// <returns>{Token: [Token] }</returns>
        [HttpPost("login")]
        [SwaggerOperation(Summary = "returns token and refresh token depending upon the passed email and password")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] UserRequest paramUser)
        {
            try
            {
                var result = await _userService.PasswordSignInAsync(paramUser.Email, paramUser.Password, false, lockoutOnFailure: false);
                
                if (result.Succeeded || result.RequiresTwoFactor)
                {
                    AspNetUsers IdentityUser = paramUser.Convert();

                    AuthenticationResult response = await _userService.GenerateAuthenticationResultForUserAsync(IdentityUser);

                    return HandleResponse(response);
                }

                return BadRequest("Wrong Username or password");
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }

        [HttpPost("refresh")]
        [SwaggerOperation(Summary = "returns updated token and refresh token depending upon the passed refresh token")]
        [AllowAnonymous]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
        {
            try
            {
                var response = await _userService.RefreshTokenAsync(request.Token, request.RefreshToken);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Sends password reset link against the email.
        /// API Path:  api/account/forgotpassword
        /// </summary>
        /// <param name="paramUser">Email</param>
        [HttpPost("forgotpassword")]
        [SwaggerOperation(Summary = "Sends password reset link against the email")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest forgotPasswordRequest)
        {
            try
            {
                var response = await _userService.ValidateUserandSendPasswordResetLink(forgotPasswordRequest.Email);

                return HandleResponse(response);

            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Returns the user id against password reset request.
        /// API Path:  api/account/resetpassword
        /// </summary>
        /// <param name="paramUser">code</param>
        [HttpGet("resetpassword")]
        [SwaggerOperation(Summary = "Returns the user id against password reset request")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromQuery] string code)
        {
            try
            {
                var response = await _userService.GetUserIdForResetPassword(code);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Returns the user id against password reset request.
        /// API Path:  api/account/resetpassword
        /// </summary>
        /// <param name="paramUser">code</param>
        [HttpPost("resetpassword")]
        [SwaggerOperation(Summary = "Updates the user's password")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest resetPasswordRequest)
        {
            try
            {
                var response = await _userService.ResetPassword(resetPasswordRequest);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Registers the user.
        /// API Path:  api/account/register
        /// </summary>
        /// <param name="paramUser">code</param>
        [HttpPost("register")]
        [SwaggerOperation(Summary = "Registers the user")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegistrationRequest registrationRequest)
        {
            try
            {
                var response = await _userService.Register(registrationRequest);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Confirms the user email against the registration process.
        /// API Path:  api/account/confirmemail
        /// </summary>
        /// <param name="paramUser">code</param>
        [HttpGet("confirmemail")]
        [SwaggerOperation(Summary = "Confirms the user email against the registration process")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string code)
        {
            try
            {
                var response = await _userService.GetRegistrationConfirmation(userId, code);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
        /// <summary>
        /// Returns the user id against password reset request.
        /// API Path:  api/account/resetpassword
        /// </summary>
        /// <param name="paramUser">code</param>
        [HttpPost("createuser")]
        [SwaggerOperation(Summary = "Creates the user against the client id assigns the role")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            try
            {
                var response = await _userService.CreateUser(createUserRequest);

                return HandleResponse(response);
            }
            catch (Exception ex)
            {
                var errorResult = ex.GetErrorResponse(_currentUser);

                return BadRequest(new
                {
                    result = errorResult
                });
            }
        }
    }
}
