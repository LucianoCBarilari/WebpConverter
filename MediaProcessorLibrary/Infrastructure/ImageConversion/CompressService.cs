using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Infrastructure.ImageConversion
{
    public class CompressService
    {
        /*// <summary>
        /// Checks the image dimensions to determine if they are within the allowed maximum width and height.
        /// Returns a tuple where the first element is a boolean indicating if the dimensions are allowed,
        /// and the second element is a message describing which dimension(s) exceeded limits (if any).
        /// </summary>
        /// <param name="img">The image whose dimensions will be checked.</param>
        /// <param name="maxWidth">The maximum allowed width of the image.</param>
        /// <param name="maxHeight">The maximum allowed height of the image.</param>
        /// <returns>A tuple with a boolean value and a string message.</returns>
        private (bool isAllowed, string message) AllowedImgSize(SixLabors.ImageSharp.Image img, int maxWidth, int maxHeight)
        {
            bool exceedsWidth = img.Width > maxWidth;
            bool exceedsHeight = img.Height > maxHeight;

            if (!exceedsWidth && !exceedsHeight)
            {
                return (true, string.Empty);
            }

            string errorMessage = string.Empty;
            if (exceedsWidth && exceedsHeight)
            {
                errorMessage = "Image dimensions exceeded: width and height.";
            }
            else if (exceedsWidth)
            {
                errorMessage = "Image width exceeded.";
            }
            else if (exceedsHeight)
            {
                errorMessage = "Image height exceeded.";
            }
            return (false, errorMessage);
        }

        /// <summary>
        /// Valida el tama�o del archivo comprobando si est� vac�o o si supera el tama�o m�ximo permitido.
        /// Devuelve una tupla donde el primer valor indica si el archivo es v�lido y el segundo valor contiene un mensaje de error en caso de serlo.
        /// </summary>
        private (bool isAllowed, string message) AllowedDataSize(IBrowserFile file, long maxSizeInBytes)
        {
            if (file.Size == 0)
            {
                return (false, "Error: The file is empty.");
            }
            if (file.Size > maxSizeInBytes)
            {
                return (false, "File size exceeded. Please upload a smaller file.");
            }
            return (true, string.Empty);
        }

        /// <summary>
        /// Compresses the given image file.
        /// Validates file size and processes the image by converting JPG, JPEG, or PNG files to WebP format,
        /// while handling GIF files appropriately.
        /// Returns a Result containing the base64 string of the processed image and the operation status.
        /// </summary>
        public async Task<Result> CompressImg(IBrowserFile file, long maxSizeInBytes, int maxWidth, int maxHeight)
        {
            // Validate the maximum allowed file size (in bytes)
            var (isSizeAllowed, sizeMessage) = AllowedDataSize(file, maxSizeInBytes);
            if (!isSizeAllowed)
            {
                return new Result
                {
                    IsSuccess = false,
                    Message = sizeMessage
                };
            }

            string defaultImg = string.Empty;

            try
            {
                // Create a stream to read the file and copy its content to a memory stream.
                using var stream = file.OpenReadStream(maxSizeInBytes);
                using var memoryStream = new MemoryStream();
                await stream.CopyToAsync(memoryStream);

                // Check if the stream is empty.
                if (stream.Length == 0)
                {
                    return new Result
                    {
                        IsSuccess = false,
                        Message = "Error loading file. The file is empty."
                    };
                }

                // Create a fresh stream for loading the image.
                using var loadStream = file.OpenReadStream(maxSizeInBytes);
                var image = await SixLabors.ImageSharp.Image.LoadAsync(loadStream);

                // Validate image dimensions
                var (isDimensionsAllowed, dimensionsMessage) = AllowedImgSize(image, maxWidth, maxHeight);
                if (!isDimensionsAllowed)
                {
                    return new Result
                    {
                        IsSuccess = false,
                        Message = dimensionsMessage
                    };
                }

                // Determine file type
                var fileType = Path.GetExtension(file.Name).ToLower();

                // Process JPG, JPEG, or PNG images by converting them to WebP.
                if (fileType == ".jpg" || fileType == ".jpeg" || fileType == ".png")
                {
                    using var imageFromMemoryStream = SixLabors.ImageSharp.Image.Load(memoryStream.ToArray());
                    using var webpStream = new MemoryStream();
                    await imageFromMemoryStream.SaveAsync(webpStream, new WebpEncoder());

                    var webpBytes = webpStream.ToArray();
                    var base64String = Convert.ToBase64String(webpBytes);
                    defaultImg = $"data:image/webp;base64,{base64String}";
                }
                // Process GIF images.
                else if (fileType == ".gif")
                {
                    var imageBytes = memoryStream.ToArray();
                    var base64String = Convert.ToBase64String(imageBytes);
                    var contentType = file.ContentType;
                    defaultImg = $"data:{contentType};base64,{base64String}";
                }
                // Return the successful result with the processed image.
                return new Result
                {
                    IsSuccess = true,
                    Value = defaultImg,
                    Message = "Image compressed successfully."
                };
            }
            catch (Exception ex)
            {
                // Return the error result if any exception occurs.
                return new Result
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }*/
    }
}
