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
            var questionTemplateCode = formData["QuestionTemplateCode"];
            var jsonFile = formData.Files.GetFile("jsonFile");

            if (jsonFile == null || string.IsNullOrEmpty(questionTemplateCode))
            {
                return BadRequest("Json file or QuestionTemplateCode is missing.");
            }

            List<QuestionTemplatesDetailDto> questionTemplateDto;
            using (var stream = jsonFile.OpenReadStream())
            using (var reader = new StreamReader(stream))
            {
                var jsonString = await reader.ReadToEndAsync();
                questionTemplateDto = JsonConvert.DeserializeObject<List<QuestionTemplatesDetailDto>>(jsonString);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var questionTemplate = new QuestionTemplate
            {
                QuestionTemplateCode = questionTemplateCode,
                CreatedDate = DateTime.UtcNow
            };

            _context.QuestionTemplates.Add(questionTemplate);
            await _context.SaveChangesAsync();

            foreach (var detailDto in questionTemplateDto)
            {
                var detail = new QuestionTemplatesDetail
                {
                    QuestionTemplateId = questionTemplate.QuestionTemplateId,
                    QId = detailDto.QID,
                    QText = detailDto.Qtext
                };

                _context.QuestionTemplatesDetails.Add(detail);
                await _context.SaveChangesAsync();

                // Thêm các QAid vào bảng phụ
                foreach (var qaid in detailDto.QAIDs)
                {
                    var detailQAid = new QuestionTemplateDetailQaid
                    {
                        QuestionTemplatesDetailId = detail.QuestionTemplatesDetailId,
                        QAid = detailDto.QID + qaid
                    };

                    _context.QuestionTemplateDetailQaids.Add(detailQAid);
                }
                await _context.SaveChangesAsync();

                var imageUrl = detailDto.ImageURL;
                if (!string.IsNullOrEmpty(imageUrl))
                {
                    var multimedia = new Multimedium
                    {
                        QuestionTemplatesDetailId = detail.QuestionTemplatesDetailId,
                        MultimediaUrl = imageUrl,
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
