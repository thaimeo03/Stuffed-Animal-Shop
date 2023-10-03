using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Options;
using Stuffed_Animal_Shop.Utilities;

namespace Stuffed_Animal_Shop.Services
{
    public class PhotoService
    {
        private readonly Cloudinary _cloudinary;
        public PhotoService(IOptions<CloudinarySetting> config)
        {
            var acc = new Account(
                config.Value.CloudName,
                config.Value.ApiKey,
                config.Value.ApiSecret
            );
            _cloudinary = new Cloudinary( acc );
        }

        public string AddPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();
            if (file.Length > 0)
            {
                using var stream = file.OpenReadStream();
                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                };
                uploadResult = _cloudinary.Upload(uploadParams);
            }

            return uploadResult.Url.ToString();
        }

        public List<string> AddPhotosAsync(List<IFormFile> files)
        {
            List<string> uploadResults = new List<string>();

            foreach (var file in files)
            {
                if (file.Length > 0)
                {
                    using var stream = file.OpenReadStream();
                    var uploadParams = new ImageUploadParams
                    {
                        File = new FileDescription(file.FileName, stream),
                        Transformation = new Transformation().Height(500).Width(500).Crop("fill").Gravity("face")
                    };
                    var uploadResult = _cloudinary.Upload(uploadParams);
                    uploadResults.Add(uploadResult.Url.ToString());
                }
            }

            return uploadResults;
        }


        public Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
