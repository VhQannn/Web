﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Web.Controllers;
using Web.DbConnection;
using Web.DTOs;

namespace Web.Pages
{
    [Authorize]
    public class CheckScoreModel : PageModel
    {
        private readonly IHttpClientFactory _clientFactory;
        public int UserId { get; set; }
        private readonly WebContext _context;
        public List<string> CurrentCourses { get; set; } = new List<string>();
        private readonly MarkReportServices _markReportServices;
        public CheckScoreModel(IHttpClientFactory clientFactory, WebContext context, MarkReportServices markReportServices)
        {
            _clientFactory = clientFactory;
            _context = context;
            _markReportServices = markReportServices;
        }
        public void OnGet()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
            CurrentCourses = _context.QuestionTemplates.Select(x => x.QuestionTemplateCode).ToList();
        }

        public async Task<ActionResult> OnPostAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                string examCode = await _markReportServices.GetTemplateCodeFromFile(file);
                var checkExistExamcode = _context.QuestionTemplates.FirstOrDefault(x => x.QuestionTemplateCode == examCode);
                if(checkExistExamcode != null)
                {
                    var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
                    UserId = userIdClaim != null ? int.Parse(userIdClaim.Value) : 1;
                    var client = _clientFactory.CreateClient();
                    var absoluteUri = "https://hotrohoctap.azurewebsites.net/api/upload";
                    using var content = new MultipartFormDataContent();
                    using var fileStream = file.OpenReadStream();
                    using var streamContent = new StreamContent(fileStream);
                    content.Add(streamContent, "file", file.FileName);
                    content.Add(new StringContent(UserId.ToString()), "userId");
                    var response = await client.PostAsync(absoluteUri, content);

                    if (response.IsSuccessStatusCode)
                    {
                        var responseUri = await response.Content.ReadFromJsonAsync<MarkReport1DTO>();
                        return RedirectToPage("/MyPayment");
                    }
                    else
                    {
                        TempData["AlertMessage"] = "Mã đề này chưa hỗ trợ";
                        return Page();
                    }
                }
                else
                {
                    TempData["AlertMessage"] = "Mã đề này chưa hỗ trợ";
                    return Page();
                }   
            }
            return Page();
        }
    }
}
