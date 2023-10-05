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

        public UploadResult AddPhotoAsync(IFormFile file)
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

            return uploadResult;
        }

        public List<UploadResult> AddPhotosAsync(List<IFormFile> files)
        {
            List<UploadResult> uploadResults = new List<UploadResult>();

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
                    uploadResults.Add(uploadResult);
                }
            }

            return uploadResults;
        }


        public Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return _cloudinary.DestroyAsync(deleteParams);
        }

        public async Task<List<DeletionResult>> DeletePhotosAsync(List<string> publicIds)
        {
            var deleteResults = new List<DeletionResult>();

            foreach (var publicId in publicIds)
            {
                var deleteParams = new DeletionParams(publicId);
                var deletionResult = await _cloudinary.DestroyAsync(deleteParams);
                deleteResults.Add(deletionResult);
            }

            return deleteResults;
        }

        public string GetPublicId(string imageUrl)
        {
            // Tách đường dẫn hình ảnh thành mảng các phần bằng "/"
            string[] urlParts = imageUrl.Split('/');

            // Lấy phần tử cuối cùng trong mảng (phần ID mong muốn)
            string imageId = urlParts.LastOrDefault().Split('.')[0];

            return imageId;
        }
    }
}
