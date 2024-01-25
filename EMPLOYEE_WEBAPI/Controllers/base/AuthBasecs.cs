using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Employee_WebAPI.Filters;
using Employee_Shared_Service.Contracts;

namespace WebApplication_WebAPI.Controllers.Base
{
    public class AuthBase : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IBaseAuth _baseAuth;
        private readonly ILoggerManager _logger;
        public AuthBase(ILoggerManager logger, IHttpContextAccessor httpContextAccessor, IBaseAuth baseAuth)
        {
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _baseAuth = baseAuth;
        }
        public async Task<string> AuthenticationAndAuthorization(List<string> permissionto)
        {
            string msg = "Success";
            var accessToken = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
            var userPrincpal = await _baseAuth.AuthenticateJwtToken(accessToken);
            if (userPrincpal == null)
            {
                msg = "Unauthorized User credentials is not a valid";

                _logger.LogWarn(msg);
                _logger.LogWarn("Remote IP Client: "+ GetRemoteIP());
                _logger.LogWarn("client useragent: " + GetUserAgent());
                _logger.LogWarn("client language: " + GetUserLanguage());
                 GetUnknown();//log custom header in a log file log unauthorized request of each Header of Each Request
                _logger.LogError(msg);
                return msg;
            }
            var userRigths = await _baseAuth.AuthorizeUser(userPrincpal, permissionto);
            if (!userRigths)
            {
                msg = "Unauthorized User access to the resource is not a valid";
                return msg;
            }
            return msg;
        }

        public string GetRemoteIP()
        {
            return _httpContextAccessor.GetRequestIP();
        }
        public string GetUserAgent()
        {
            return _httpContextAccessor.UserAgent();
        }
        public string GetUserLanguage()
        {
            return _httpContextAccessor.ClientLanguage();
        }
        public void GetUnknown()
        {
            _httpContextAccessor.Unknown();
        }

    }
}
