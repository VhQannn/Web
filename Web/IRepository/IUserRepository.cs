using Web.DbConnection;
using Web.Models;

namespace Web.IRepository
{
    public interface IUserRepository
    {
        User Login(string username, string password);
        bool Register(RegisterDTO userDTO);
        List<User> GetAll();
        User UpdateRole(int userId, string role);
    }
}
