using Employee_Shared_Service_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Employee_DataAccessLayer.Repository
{
    public interface IUserRepository
    {
        public Task<IEnumerable<User>> GetUsers();
        public Task<int> InsertUser(User user);
        public Task<IEnumerable<Role>> GetUserRoleByUserid(string useremail);
        public Task<bool> ValidateUser(string userName, string password);
        public Task<User> FindUser(string userName, string password);
    }
}
