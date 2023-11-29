using Web.DbConnection;
using Web.Models;

namespace Web.IRepository
{
    public interface IUserRepository
    {
        User Login(string username, string password);
        bool Register(RegisterDTO userDTO, bool isVerify);
        List<User> GetAll();
        User UpdateRole(int userId, string role);
        Task<User> ChangePassword(string username, string password);
        string RandomPassword();
    }
}
