using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Ultil;

namespace Web.Controllers
{
    [Route("api/upload")]
    [ApiController]
    public class UploadFileController : ControllerBase
    {
        private readonly UploadFile _uploadFile;
        public UploadFileController(UploadFile uploadFile)
        {
            _uploadFile = uploadFile;   
        }

        [HttpPost()]
        public async Task<ActionResult<string>> Upload(IFormFile file)
        {
            var result = await _uploadFile.UploadFileToCloud(file);
            return result.SecureUri.ToString();
        }
    }
}
