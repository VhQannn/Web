using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.DbConnection;
using Web.DTOs;
using Web.Models;
using Web.Util;

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
                        MarkReportId = check.Entity.MarkReportId,
                        MultimediaType = "file"

                    };
                    await _context.Multimedia.AddAsync(multimedia);
                    isSuccess = await _context.SaveChangesAsync() > 0;
                    
                    if (isSuccess)
                    {
                        var payment = new Payment
                        {
                            UserId = userId,
                            Amount = 50000,
                            PaymentDate = DateTime.UtcNow,
                            RelatedId = check.Entity.MarkReportId,
                            ServiceType = "Check-Score",
                            Status = "PENDING"
                        };

                        await _context.Payments.AddAsync(payment);
                        isSuccess = await _context.SaveChangesAsync() > 0;
                        if (isSuccess)
                        {
                            var respone = new MarkReport1DTO
                            {
                                MarkReportId = check.Entity.MarkReportId,
                                CreatedBy = check.Entity.CreatedBy,
                                CreatedDate = check.Entity.CreatedDate,
                                UpdatedBy = check.Entity.UpdatedBy,
                                UpdatedDate = check.Entity.UpdatedDate,
                                MarkScore = check.Entity.MarkScore,
                                UserId = userId,
                                Uri = result.SecureUri.ToString()
                            };
                            return Ok(respone);
                        }
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
