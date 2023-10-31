using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Web.DbConnection;

namespace Web.Pages
{
    public class CreatePostModel : PageModel
    {
        private readonly Web.DbConnection.WebContext _context;
        private readonly IHubContext<PostHub> _postHub;

        public CreatePostModel(Web.DbConnection.WebContext context, IHubContext<PostHub> postHub)
        {
            _context = context;
            _postHub = postHub;
        }

        public IActionResult OnGet()
        {
        ViewData["UserId"] = new SelectList(_context.Users, "UserId", "UserId");
            return Page();
        }

        [BindProperty]
        public Post Post { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
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
