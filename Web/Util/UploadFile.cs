using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Web.Util
{
    public class UploadFile
    {
        private readonly IConfiguration _configuration;
        public UploadFile(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        public async Task<UploadResult> UploadFileToCloud(IFormFile file)
        {
            string cloudName = _configuration["Cloudinary:CloudName"];
            string apiKey = _configuration["Cloudinary:ApiKey"];
            string apiSecret = _configuration["Cloudinary:ApiSecret"];

            Cloudinary cloudinary = new Cloudinary(new Account(cloudName, apiKey, apiSecret));
            using (var stream = file.OpenReadStream())
            {
                var uploadParams = new RawUploadParams()
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "test"
                };

                return await cloudinary.UploadAsync(uploadParams);
            }
        }


    }
}
