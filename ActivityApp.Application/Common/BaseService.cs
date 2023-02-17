using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ActivityApp.Domain.Data;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ActivityApp.Application.Common
{
    public class BaseService
    {
        public BaseService(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            this._httpContextAccessor = httpContextAccessor;
            this.configuration = configuration;

            _identity = _httpContextAccessor?.HttpContext?.User?.Identity as ClaimsIdentity;
            _claims = _identity?.Claims;

            //TODO: static constants.
            _currentUser.Id = _claims?.Where(x => x.Type == "UserId").FirstOrDefault()?.Value;
            _currentUser.ClientId = Convert.ToInt32(_claims?.Where(x => x.Type == "ClientId").FirstOrDefault()?.Value);
            _currentUser.UserName = _claims?.Where(x => x.Type == JwtRegisteredClaimNames.Jti).FirstOrDefault()?.Value;
        }

        #region Data memebrs and Properties

        public readonly IHttpContextAccessor _httpContextAccessor;
        public readonly ClaimsIdentity _identity;
        public IEnumerable<Claim> _claims { get; }

        public AspNetUsers _currentUser = new AspNetUsers();

        public IConfiguration configuration;

        #endregion
    }
}
