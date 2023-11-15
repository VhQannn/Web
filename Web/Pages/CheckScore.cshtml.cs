using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages
{
    public class CheckScoreModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;

        public CheckScoreModel(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            
        }
        public void OnGet()
        {
        }

        public async Task<ActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                var client = _clientFactory.CreateClient();
                var absoluteUri = "https://localhost:7247/api/upload";
                using var content = new MultipartFormDataContent();
                using var fileStream = file.OpenReadStream();
                using var streamContent = new StreamContent(fileStream);
                content.Add(streamContent, "file", file.FileName);

                var response = await client.PostAsync(absoluteUri, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseUri = await response.Content.ReadAsStringAsync();
                    return RedirectToPage("/CheckResult", new { uri = responseUri });
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
