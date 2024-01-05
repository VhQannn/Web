using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Web.DbConnection;
using Web.IRepository;
using Microsoft.AspNetCore.Mvc;
namespace Web.Pages.Admin
{
	[Authorize(Roles = "Admin")]
	public class UserManagementModel : PageModel
    {
        private readonly IUserRepository _userRepository;
        
        public List<User> Users { get; set; }

        public RoleEnum SelectedRole { get; set; }
        
        public UserManagementModel(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public void OnGet()
        {
            Users = _userRepository.GetAll();
        }

        public List<User> SearchUser(string username)
        {
            var list = _userRepository.GetAll();
            var searchResult = list.Where(x => x.Username.Contains(username)).ToList();
            return searchResult;
        }
   
    }
}
