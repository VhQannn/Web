using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Web.DbConnection;

namespace Web.Pages
{
    [Authorize]
    public class CreatePostModel : PageModel
    {
        private readonly WebContext _context;

        public CreatePostModel(WebContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            ViewData["PostCategoryId"] = new SelectList(_context.PostCategories, "PostCategoryId", "PostCategoryName");
            return Page();
        }

        [BindProperty]
        public Post Post { get; set; } = default!;

        /*[BindProperty]
        public string StartTime { get; set; } = default!;

        [BindProperty]
        public string EndTime { get; set; } = default!;*/


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            var currentUserName = User.Identity.Name;
            var currentUser = _context.Users.FirstOrDefault(u => u.Username == currentUserName);
            Post.PostDate = DateTime.Now;
            Post.Status = "pending";
            Post.TimeSlot = "";
            Post.User = currentUser;

            if (!ModelState.IsValid || _context.Posts == null || Post == null)
            {
                return Page();
            }
            _context.Posts.Add(Post);
            await _context.SaveChangesAsync();

            //Này e làm signalr cho cái post á nha 
            var postHub = (IHubContext<PostHub>)HttpContext.RequestServices.GetService(typeof(IHubContext<PostHub>));
            await postHub.Clients.All.SendAsync("UpdatePosts");
            ////

            return RedirectToPage("./Index");
        }
    }
}
