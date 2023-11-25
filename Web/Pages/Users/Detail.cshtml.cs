using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Web.DbConnection;
using Web.Models;
using Web.Repository;

namespace Web.Pages.Users
{
    public class DetailModel : PageModel
    {
        private readonly WebContext _context;
        private const int PageSize = 5; // Number of comments per page
        // Add properties to hold the data
        public string Username { get; set; }
        public string? UserType { get; set; }
        public string? Facebook { get; set; }
        public string? Email { get; set; }
        public int UserId { get; set; }
        public double AverageRating { get; set; }
        public bool? IsVerify { get; set; }
        public List<RatingCommentDTO> Comments { get; set; }
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public DetailModel(WebContext context)
        {
            _context = context;
            Comments = new List<RatingCommentDTO>();
        }

        public async Task<IActionResult> OnGetAsync(int uId, int currentPage = 1)
        {
            CurrentPage = currentPage;
            // Query the database for the user with the specified userId
            var user = await _context.Users
                                     .Where(u => u.UserId == uId)
                                     .Select(u => new
                                     {
                                         u.UserId,
                                         u.Username,
                                         u.UserType,
                                         u.Facebook,
                                         u.IsVerify,
                                         u.Email,
                                     })
                                     .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }
            var ratings = _context.Ratings.Where(r => r.SupporterId == uId);

            // Check if there are any ratings and calculate average
            if (await ratings.AnyAsync())
            {
                AverageRating = await ratings.AverageAsync(r => r.RatingValue ?? 0);
                int totalComments = await _context.Ratings.CountAsync(r => r.SupporterId == uId);
                TotalPages = (int)Math.Ceiling(totalComments / (double)PageSize);

                Comments = await _context.Ratings
                                 .Where(r => r.SupporterId == uId)
                                 .Include(r => r.Rater) // Assuming 'Rater' is the navigation property in Rating
                                 .Select(r => new RatingCommentDTO
                                 {
                                     RaterName = r.Rater.Username, // Replace 'Username' with actual property name
                                     Comment = r.Comments,
                                     RatingValue = (double) r.RatingValue,
                                     RatingDate = r.RatingDate
                                 })
                                 .Skip((CurrentPage - 1) * PageSize)
                                         .Take(PageSize)
                                         .ToListAsync();
            }
            else
            {
                AverageRating = 0; // Or any default value you see fit
            }

            // Assign the values
            UserId = user.UserId;
            Username = user.Username;
            UserType = user.UserType;
            Facebook = user.Facebook;
            IsVerify = user.IsVerify;
            Email = user.Email;
            return Page();
        }
    }

}

