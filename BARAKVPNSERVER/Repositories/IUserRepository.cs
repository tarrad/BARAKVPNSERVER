using BARAKVPNSERVER.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BARAKVPNSERVER.Repositories
{
    public interface IUserRepository
    {
        Task<User> Login(string username, string password);
        Task<List<string>> Register(GroupWeb group);
        Task<bool> UserExists(string username);
        Task<User> GetActiveAdminUser(int Id);
        Task<List<UserAdd>> GetAllUsersByGroupId(int Id);
        Task<List<string>> AddUser(UserAdd user);
        Task<List<string>> EditUser(User user, UserAdd userToEdit);
        Task<List<string>> DeleteUser(string username, User user);
    }
}
