using Microsoft.AspNetCore.SignalR;
using Web.IRepository;

namespace Web
{
    public class UserUpdateHub:Hub
    {
        private readonly IUserRepository _userRepository;
        
        public UserUpdateHub(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task UpdateUserType(int userId, string newUserType)
        {
            var check = _userRepository.UpdateRole(userId, newUserType);
            await Clients.All.SendAsync("UserTypeUpdated", check.UserId, check.UserType);
        }
    }
}
