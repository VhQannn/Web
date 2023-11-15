using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Web.DbConnection;
using Web.Models;
using Web.Util;

namespace Web.Controllers
{
    [Route("api/question-template")]
    [ApiController]
    public class QuestionTemplateController : Controller
    {
        private readonly WebContext _context;

        private readonly UploadFile _uploadFile;

        public QuestionTemplateController(WebContext context, UploadFile uploadFile)
        {
            _context = context;
            _uploadFile = uploadFile;
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuestionTemplateWithDetails()
        {
            var formData = await Request.ReadFormAsync();
            var questionTemplateJson = formData["questionTemplate"];
            var questionTemplateDto = JsonConvert.DeserializeObject<QuestionTemplateDto>(questionTemplateJson);

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionTemplate = new QuestionTemplate
            {
                QuestionTemplateCode = questionTemplateDto.QuestionTemplateCode,
                CreatedDate = DateTime.UtcNow
            };

            _context.QuestionTemplates.Add(questionTemplate);
            await _context.SaveChangesAsync();

            for (int i = 0; i < questionTemplateDto.QuestionTemplatesDetails.Count; i++)
            {
                var detailDto = questionTemplateDto.QuestionTemplatesDetails[i];
                var detail = new QuestionTemplatesDetail
                {
                    QuestionTemplateId = questionTemplate.QuestionTemplateId,
                    QId = detailDto.QId,
                    QAid = detailDto.QAid,
                    QText = detailDto.QText
                };

                _context.QuestionTemplatesDetails.Add(detail);
                await _context.SaveChangesAsync();

                var imageFile = formData.Files.GetFile($"{detailDto.QId}");
                if (imageFile != null)
                {

                    var imageUrl = await _uploadFile.UploadFileToCloud(imageFile);

                    var multimedia = new Multimedium
                    {
                        QuestionTemplatesDetailId = detail.QuestionTemplatesDetailId,
                        MultimediaUrl = imageUrl.SecureUri.ToString(),
                        MultimediaType = "image",
                        CreatedDate = DateTime.UtcNow
                    };

                    _context.Multimedia.Add(multimedia);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { Message = "Question template và chi tiết đã được tạo thành công." });
        }
    }


}
