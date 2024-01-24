using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Employee_Shared_Service_Model.Model;

namespace Employee_Service.Contracts
{
    public interface IAuthService
    {
        Task<int> RegisterUser(User userModel);
        Task<User> FindUser(string userName, string password);
        Task<bool> ValidateUser(string userName, string password);
        Task<string> GenerateToken(string userName);
        ClaimsPrincipal GetPrincipal(string token);
        Task<IEnumerable<Role>> GetUserRoles(string userName);
    }
}
