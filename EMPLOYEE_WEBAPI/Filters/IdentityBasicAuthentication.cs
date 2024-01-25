using Employee_Service.Contracts;
using Employee_WebAPI.Filters;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;


namespace WebApplication_WebAPI.Filters
{
    public class IdentityBasicAuthentication : IBaseAuth
    {

        private readonly IAuthService _authService;

        public IdentityBasicAuthentication(IAuthService authService)
        {
            _authService = authService;
        }


        private bool ValidateToken(string token, out string username)
        {
            username = null;

            var simplePrinciple = _authService.GetPrincipal(token);
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        public async Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            try
            {
                token = token.Split(' ')[1];
                string username;
                if (ValidateToken(token, out username))
                {
                    // based on username to get more information from database 
                    // in order to build local identity

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username)
                    };

                    // Add more claims if needed: Roles, ...
                    var rle = await _authService.GetUserRoles(username);
                    foreach (var item in rle)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.Name));
                    }

                    var identity = new ClaimsIdentity(claims, "Jwt");
                    IPrincipal user = new ClaimsPrincipal(identity);
                    //
                    return user;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public Task<bool> AuthorizeUser(IPrincipal user, List<string> permissionto)
        {
            foreach (var per in permissionto)
            {
                if (user.IsInRole(per))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public bool AuthorizeUserClaim(IPrincipal user, List<string> permissionto)
        {
            foreach (var per in permissionto)
            {
                if (user.IsInRole(per))
                {
                    return true;
                }
            }
            return false;
        }
    }
    public class BaseAuth : IBaseAuth
    {
        private readonly IAuthService _authService;

        public BaseAuth(IAuthService authService)
        {
            _authService = authService;
        }


        private bool ValidateToken(string token, out string username)
        {
            username = null;

            var simplePrinciple = _authService.GetPrincipal(token);
            var identity = simplePrinciple.Identity as ClaimsIdentity;

            if (identity == null)
                return false;

            if (!identity.IsAuthenticated)
                return false;

            var usernameClaim = identity.FindFirst(ClaimTypes.Name);
            username = usernameClaim?.Value;

            if (string.IsNullOrEmpty(username))
                return false;

            // More validate to check whether username exists in system

            return true;
        }

        public async Task<IPrincipal> AuthenticateJwtToken(string token)
        {
            try
            {
                token = token.Split(' ')[1];
                string username;
                if (ValidateToken(token, out username))
                {
                    // based on username to get more information from database 
                    // in order to build local identity

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, username)
                    };

                    // Add more claims if needed: Roles, ...
                    var rle = await _authService.GetUserRoles(username);
                    foreach (var item in rle)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, item.Name));
                    }

                    var identity = new ClaimsIdentity(claims, "Jwt");
                    IPrincipal user = new ClaimsPrincipal(identity);
                    //
                    return user;
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public Task<bool> AuthorizeUser(IPrincipal user, List<string> permissionto)
        {
            foreach (var per in permissionto)
            {
                if (user.IsInRole(per))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public bool AuthorizeUserClaim(IPrincipal user, List<string> permissionto)
        {
            foreach (var per in permissionto)
            {
                if (user.IsInRole(per))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
