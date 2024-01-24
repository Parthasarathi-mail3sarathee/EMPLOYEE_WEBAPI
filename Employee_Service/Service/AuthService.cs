using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Employee_Shared_Service_Model.Model;
using Employee_Service.Contracts;
using Employee_Shared_Service.Contracts;
using Employee_Shared_Service;
using Employee_DataAccessLayer.Repository;

namespace Employee_Service.Service
{
    public sealed class AuthService : IAuthService
    {
        private string key { get; set; }
        private readonly ILoggerManager _logger;
        private readonly IUserRepository _userRepository;
        public AuthService(ILoggerManager logger, IUserRepository userRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            key = "my_secret_key_12345";
        }

        public Task<int> RegisterUser(User userModel)
        {
            var users = _userRepository.GetUsers().Result;
            if (users.Count() > 0)
            {
                var res = _userRepository.InsertUser(userModel);
                return res;
            }
            return Task.FromResult(-1);
        }

        public Task<IEnumerable<Role>> GetUserRoles(string userName)
        {
            var roles = _userRepository.GetUserRoleByUserid(userName);

            return roles;
        }


        public Task<User> FindUser(string userName, string password)
        {
            var user = _userRepository.FindUser(userName , password);
            return user;
        }

        public Task<bool> ValidateUser(string userName, string password)
        {
            var verifieduser = _userRepository.ValidateUser(userName , password);
            return verifieduser;
        }

        public Task<string> GenerateToken(string userName)
        {
            try
            {
                //byte[] key = Convert.FromBase64String(Secret);
                //Secret key which will be used later during validation    
                var issuer = "http://mysite.com";  //normally this will be your site URL    

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                //Create a List of Claims, Keep claims name short    
                var permClaims = new List<Claim>();
                permClaims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
                permClaims.Add(new Claim("name", userName));
                permClaims.Add(new Claim(ClaimTypes.Name, userName));

                //Create Security Token object by giving required parameters    
                var token = new JwtSecurityToken(issuer, //Issure    
                                issuer,  //Audience    
                                permClaims,
                                expires: DateTime.Now.AddDays(1),
                                signingCredentials: credentials);
                var jwt_token = new JwtSecurityTokenHandler().WriteToken(token);
                return Task.FromResult(jwt_token);
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: " + ex.Message);
                _logger.LogError("StackTrace: " + ex.StackTrace);
                return Task.FromResult<string>(null);
            }
        }

        public ClaimsPrincipal GetPrincipal(string token)
        {
            try
            {
                JwtSecurityTokenHandler tokenHandler = new JwtSecurityTokenHandler();
                JwtSecurityToken jwtToken = (JwtSecurityToken)tokenHandler.ReadToken(token);
                if (jwtToken == null) return null;
                byte[] keybyte = Encoding.UTF8.GetBytes(key);

                var IssuerSigningKey = new SymmetricSecurityKey(keybyte);

                var credentials = new SigningCredentials(IssuerSigningKey, SecurityAlgorithms.HmacSha256);

                var tokenSecure = tokenHandler.ReadToken(token) as SecurityToken;

                TokenValidationParameters parameters = new TokenValidationParameters()
                {
                    RequireExpirationTime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = IssuerSigningKey,
                    ValidateIssuerSigningKey = true
                };
                ClaimsPrincipal principal = tokenHandler.ValidateToken(token, parameters, out tokenSecure);

                _logger.LogInfo(tokenSecure.ToString());
                return principal;
            }
            catch (Exception ex)
            {
                _logger.LogError("Message: " + ex.Message);
                _logger.LogError("StackTrace: " + ex.StackTrace);
                return null;
            }
        }


    }
}
