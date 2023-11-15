using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.DTOs;

namespace Web.Pages
{
    [Authorize]
    public class CheckScoreModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public int UserId { get; set; }
        public CheckScoreModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            
        }
        public void OnGet()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
        }

        public async Task<ActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
                var client = _clientFactory.CreateClient();
                var absoluteUri = "https://localhost:7247/api/upload";
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                content.Add(streamContent, "file", file.FileName);
                content.Add(new StringContent(UserId.ToString()), "userId");
                var response = await client.PostAsync(absoluteUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseUri = await response.Content.ReadFromJsonAsync<MarkReportDTO>();
                    return RedirectToPage("/CheckResult", new { uri = responseUri.Uri, id = responseUri.MarkReportId });
                }
                else
                {
                    return Page();
                }
            }
            return Page();
        }
    }
}