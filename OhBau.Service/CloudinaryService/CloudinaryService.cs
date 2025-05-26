using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using OhBau.Model.Payload.Response;

namespace OhBau.Service.CloudinaryService
{
    public class CloudinaryService(Cloudinary _cloudinary) : ICloudinaryService
    {
        public async Task<BaseResponse<string>> Upload(IFormFile file)
        {
            if (file == null)
            {
                return new BaseResponse<string>
                {
                    status = StatusCodes.Status400BadRequest.ToString(),
                    message = "File not found",
                    data = null
                };
            }

            using(var stream = file.OpenReadStream())
            {
                var uploadParam = new ImageUploadParams
                {
                    File = new FileDescription(file.Name, stream),
                    Folder = "EposhBooking",
                    PublicId = Guid.NewGuid().ToString(),
                    Transformation = new Transformation().Quality("auto:low")
                                                         .FetchFormat("webp")
                                                         .Width(1024)
                                                         .Crop("limit")
                };


                var uploadResult = await _cloudinary.UploadAsync(uploadParam);
                if (uploadResult.StatusCode == HttpStatusCode.OK)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Upload Success",
                        data = uploadResult.SecureUrl.ToString()
                    };
                }
                else
                {
                    throw new Exception("Failed to upload image to Cloudinary.");
                }
            }
        }

        public async Task<BaseResponse<string>> UploadVideo(IFormFile file)
        {
            try
            {
                var uploadParams = new VideoUploadParams
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    Folder = "EposhBooking",
                    PublicId = Guid.NewGuid().ToString(),
                    EagerTransforms = new List<Transformation>
            {
                new EagerTransformation().Width(300).Height(300).Crop("pad").AudioCodec("none"),
                new EagerTransformation().Width(160).Height(100).Crop("crop").Gravity("south").AudioCodec("none")
            },
                    EagerAsync = true,
                    EagerNotificationUrl = "https://your-callback-url.com/notify" 
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);

                if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Video uploaded successfully",
                        data = uploadResult.SecureUrl.ToString()
                    };
                }
                else
                {
                    return new BaseResponse<string>
                    {
                        status = StatusCodes.Status200OK.ToString(),
                        message = "Video upload failed: " + uploadResult.Error?.Message,
                        data = null,
                    };
                }
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>
                {
                    status = StatusCodes.Status200OK.ToString(),
                    message = $"An error occurred: {ex.Message}",
                    data = null
                };
            }
        }
    }
}
