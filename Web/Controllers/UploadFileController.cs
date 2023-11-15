using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.DbConnection;
using Web.DTOs;
using Web.Ultil;

namespace Web.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly UploadFile _uploadFile;
        private readonly WebContext _context;
        public UploadFileController(UploadFile uploadFile, WebContext context)
        {
            _uploadFile = uploadFile;
            _context = context;
        }

        [HttpPost()]
        public async Task<ActionResult<string>> Upload(IFormFile file, [FromForm] int userId)
        {
            try
            {
                var result = await _uploadFile.UploadFileToCloud(file);
                var mark = new MarkReport
                {
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                    MarkScore = 0,
                    UserId = userId,

                };
                var check = await _context.MarkReports.AddAsync(mark);
                var isSuccess = await _context.SaveChangesAsync() > 0;
                if (isSuccess)
                {
                    var multimedia = new Multimedium
                    {
                        CreatedDate = DateTime.UtcNow,
                        UpdatedDate = DateTime.UtcNow,
                        MultimediaUrl = result.SecureUri.ToString(),
                        UserId = userId,
                        MarkReportId = check.Entity.MarkReportId,
                        MultimediaType = "file"

                    };
                    await _context.Multimedia.AddAsync(multimedia);
                    isSuccess = await _context.SaveChangesAsync() > 0;
                    
                    if (isSuccess)
                    {
                        var respone = new MarkReportDTO
                        {
                            MarkReportId = check.Entity.MarkReportId,
                            CreatedBy = check.Entity.CreatedBy,
                            CreatedDate = check.Entity.CreatedDate,
                            UpdatedBy = check.Entity.UpdatedBy,
                            UpdatedDate = check.Entity.UpdatedDate,
                            MarkScore = check.Entity.MarkScore,
                            UserId = userId,
                        };
                        return Ok(respone);
                    }
                }

                return NotFound();

            }
            catch (Exception)
            {

                return NotFound();
            }

        }
    }
}
