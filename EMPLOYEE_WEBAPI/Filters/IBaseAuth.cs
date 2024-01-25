using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;

namespace Employee_WebAPI.Filters
{
    public interface IBaseAuth
    {
        Task<IPrincipal> AuthenticateJwtToken(string token);
        Task<bool> AuthorizeUser(IPrincipal user, List<string> permissionto);
        bool AuthorizeUserClaim(IPrincipal user, List<string> permissionto);
    }
}
