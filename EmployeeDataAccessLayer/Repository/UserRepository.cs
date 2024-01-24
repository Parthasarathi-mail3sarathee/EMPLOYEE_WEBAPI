using Employee_Shared_Service_Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Employee_Shared_Service;

namespace Employee_DataAccessLayer.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly DapperContext _context;
        public UserRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> GetUsers()
        {
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                return users.ToList();
            }
        }

        public async Task<int> InsertUser(User usr)
        {
            var sql = String.Format("INSERT INTO Users ([name],[password],[Email],[IsActive],[createdby],[createdDate],[updatedby],[updatedDate]) values ('{0}','{1}','{2}','{3}','-1', getdate(),'-1', getdate())", usr.Name, SecurePasswordHasher.Hash(usr.Password), usr.Email, usr.IsActive);
            using (var connection = _context.CreateConnection())
            {
                var affectedRows = connection.Execute(sql);

                Console.WriteLine($"Affected Rows: {affectedRows}");
                return affectedRows;
            }
        }

        public async Task<IEnumerable<Role>> GetUserRoleByUserid(string useremail)
        {
            var sql = String.Format("SELECT r.* FROM Users as u inner join userrole as ur on ur.userid=u.id inner join roles as r on r.id= ur.roleid where u.email='{0}'", useremail);
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<Role>(sql);
                return users.ToList();
            }
        }

        public async Task<bool> ValidateUser(string userName, string password)
        {
            //SELECT u.* FROM Users as u where u.email='sankar@test.com' and  u.password='$MYHASH$V1$10000$FgmAy/FK2WM16yUegb2kZg28NsvuI2Hs8DqFNl9/1U9rBLZN'/


            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                users.ToList();
                var res = users.FirstOrDefault(e => e.Email == userName && SecurePasswordHasher.Verify(password, e.Password));
                if (res == null) return false;
                else return true;
            }

        }

        public async Task<User> FindUser(string userName, string password)
        {
            //SELECT u.* FROM Users as u where u.email='sankar@test.com' and  u.password='$MYHASH$V1$10000$FgmAy/FK2WM16yUegb2kZg28NsvuI2Hs8DqFNl9/1U9rBLZN'/
            var query = "SELECT * FROM Users";
            using (var connection = _context.CreateConnection())
            {
                var users = await connection.QueryAsync<User>(query);
                users.ToList();
                var res = users.FirstOrDefault(e => e.Email == userName && SecurePasswordHasher.Verify(password, e.Password));
                if (res == null) return res;
                else return res;
            }
        }

    }
}
