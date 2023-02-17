using ActivityApp.Application.ApplicationContracts.Responses;
using ActivityApp.Application.Core.ApplicationContracts;
using ActivityApp.Application.Core.Exceptions;
using ActivityApp.Domain.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace ActivityApp.Application.Core.APIController
{
    /// <summary>
    /// APIBaseController class inhertits ControllerBase s serves as base class for all controllers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public abstract class APIBaseController : ControllerBase
    {
        public APIBaseController(IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ILogger logger)
        {
            _httpContextAccessor = httpContextAccessor;
            Configuration = configuration;
            Logger = logger;

            _identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            _claims = _identity?.Claims;

            _currentUser.Id = _claims?.Where(x => x.Type == "UserId").FirstOrDefault()?.Value;
            _currentUser.ClientId = Convert.ToInt32(_claims?.Where(x => x.Type == "ClientId").FirstOrDefault()?.Value);
            _currentUser.UserName = _claims?.Where(x => x.Type == "username").FirstOrDefault()?.Value;
        }

        #region Properties and Data Members

        private readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ClaimsIdentity _identity;
        public IEnumerable<Claim> _claims { get; }
        public AspNetUsers _currentUser = new AspNetUsers();
        public IConfiguration Configuration { get; }
        public ILogger Logger { get; }

        #endregion

        /// <summary>
        /// Heartbeatprovides API to return Test Message.
        /// API Path:  api/ControllerName/Heartbeat
        /// </summary>
        /// <param name="requestDTO"></param>
        /// <returns></returns>
        [HttpGet("Heartbeat")]
        public string Heartbeat([FromQuery] BaseRequest requestDTO)
        {
            return "Hello Test at: " + DateTime.UtcNow;
        }

        #region helper methods
        protected IActionResult HandleResponse<T>(T response) where T : IErrorResponse
        {

            if (response.OriginalException != null)
            {
                var errorResult = response.OriginalException.GetErrorResponse(_currentUser);

                for (int i = 0; i < response.APIErrors.Count; i++)
                {
                    response.APIErrors[i] = response.APIErrors[i] + errorResult.ErrorID;
                }
            }

            if (response.APIErrors?.Count == 0 && response.Errors?.Count == 0)
            {
                response.IsSuccess = true;
                return Ok(response);
            }
            else
            {
                return BadRequest(response);
            }
        }
        #endregion
    }
}
