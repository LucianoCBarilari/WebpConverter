using WebpConverter.Common.Enums;
using WebpConverter.Common.Results;
using WebpConverter.Infrastructure.FileSystem;

namespace WebpConverter.Feature.DeleteImage;

public class DeleteImageHandler(IFileService fileService) : IDeleteImageHandler
{
    /// <summary>
    /// Deletes an image file using the Result Pattern.
    /// </summary>
    public ResultMedia Delete(string folderPath, string fileName)
    {
        if (string.IsNullOrWhiteSpace(folderPath))
            return ResultMedia.Fail(ErrorCode.PathEmpty);

        if (string.IsNullOrWhiteSpace(fileName))
            return ResultMedia.Fail(ErrorCode.FileNameEmpty);
            

        fileName = Path.GetFileName(fileName);

        return fileService.DeleteFile(folderPath, fileName);
    }
}
