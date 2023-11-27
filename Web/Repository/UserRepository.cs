using Microsoft.EntityFrameworkCore;
using System;
using System.Security.Cryptography;
using System.Text;
using Web.DbConnection;
using Web.IRepository;
using Web.Models;

namespace Web.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly WebContext _context;
        public UserRepository(WebContext context)
        {
            _context = context;
        }

        public List<User> GetAll()
        {
            return _context.Users.ToList();
        }


        public User Login(string username, string password)
        {
            string hashedPassword = HashPassword(password);
            User user = _context.Users.SingleOrDefault(x => x.Username == username && x.Password == hashedPassword);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public bool IsExist(string username)
        {
            return _context.Users.Any(x => x.Username == username);
        }

        public bool Register(RegisterDTO userDTO)
        {
            if (IsExist(userDTO.Username))
            {
                return false;
            }
            User user = new User
            {
                Username = userDTO.Username,
                Password = HashPassword(userDTO.Password),
                Email = userDTO.Email,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.UtcNow,
                UserType = "Customer",
                Facebook = ""
            };
            _context.Users.Add(user);
            return _context.SaveChanges() > 0;
        }

        public User UpdateRole(int userId, string role)
        {
            var user = _context.Users.SingleOrDefault(x => x.UserId == userId);
            user.UserType = role;
            _context.SaveChanges();
            return user;
        }

        private string HashPassword(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public string RandomPassword()
        {
            Random random = new Random();
            int length = 10;
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*()";
            string password = new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());

            return password;
        }


        public async Task<User> ChangePassword(string username, string password)
        {
            var user = await _context.Users.SingleOrDefaultAsync(x => x.Username.Equals(username));
            user.Password = HashPassword(password);
            try
            {
                var result = await _context.SaveChangesAsync();
                return result > 0 ? user : null;
            }
            catch (Exception ex)
            {
                Console.Write(ex.ToString());   
                return null;
            }
        }
    }
}
