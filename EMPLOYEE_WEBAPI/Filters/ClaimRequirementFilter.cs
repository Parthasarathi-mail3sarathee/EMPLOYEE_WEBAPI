using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Employee_Shared_Service.Contracts;
using Employee_WebAPI.Filters;

namespace Employee_WebAPI.Filters
{
    public class ClaimRequirementAttribute : TypeFilterAttribute
    {
        public ClaimRequirementAttribute(string[] claims) : base(typeof(ClaimRequirementFilter))
        {
            Arguments = new object[] { claims };
        }
    }

    public class ClaimRequirementFilter : IAuthorizationFilter
    {
        readonly string[] _claim;
        //private readonly ILoggerManager _logger;

        public ClaimRequirementFilter(string[] claim)
        {
            _claim = claim;
            //_logger = new LoggerManager();
            //_baseAuth = new IdentityBasicAuthentication(new AuthService(_logger));
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            string msg = "Unauthorized User credentials is not a valid";
            var _logger = (ILoggerManager)context.HttpContext.RequestServices.GetService(typeof(ILoggerManager));
            var _baseAuth = (IBaseAuth)context.HttpContext.RequestServices.GetService(typeof(IBaseAuth));
            var accessToken = context.HttpContext.Request.Headers["Authorization"];
            var userPrincpal =  _baseAuth.AuthenticateJwtToken(accessToken).Result;
            if (userPrincpal == null)
            {
                var _httpContextAccessor = new HttpContextAccessor();
                _httpContextAccessor.HttpContext = context.HttpContext;
                _logger.LogWarn(msg);
                _logger.LogWarn("Remote IP Client: " + _httpContextAccessor.GetRequestIP());
                _logger.LogWarn("client useragent: " + _httpContextAccessor.UserAgent());
                _logger.LogWarn("client language: " + _httpContextAccessor.ClientLanguage());
                _httpContextAccessor.Unknown();//log custom header in a log file log unauthorized request of each Header of Each Request
                _logger.LogError(msg);
                context.Result = new UnauthorizedResult();
                return;
            }
            var userRigths =  _baseAuth.AuthorizeUserClaim(userPrincpal, _claim.ToList());
            if (!userRigths)
            {
                context.Result = new NoContentResult();
                return;
            }

            //var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
            //if (!hasClaim)
            //{
            //    context.Result = new ForbidResult();
            //}
        }
    }


}
