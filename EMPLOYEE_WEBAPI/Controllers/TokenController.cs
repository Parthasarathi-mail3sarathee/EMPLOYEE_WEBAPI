using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Employee_Shared_Service.Service;
using Employee_Shared_Service_Model.Model;
using Employee_Service.Contracts;

namespace EMPLOYEE_WEBAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("AllowAllHeaders")]
    public class TokenController : ControllerBase
    {
        // This is naive endpoint for demo, it should use Basic authentication
        // to provide token or POST request

        private readonly IAuthService _authService;

        public TokenController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Post(Login login)
        {
            if (CheckUser(login.username, login.password))
            {
                var tok = await _authService.GenerateToken(login.username);
                return Ok(new { token = tok });
            }

            return Unauthorized();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("RegisterUser")]
        public async Task<IActionResult> RegisterUser(User user)
        {
            var registereduser = await _authService.RegisterUser(user);
            return Ok(registereduser);
        }

        [HttpGet]
        public bool CheckUser(string username, string password)
        {
            // should check in the database
            var valid = _authService.ValidateUser(username, password).Result;
            return valid;
        }
    }
}
