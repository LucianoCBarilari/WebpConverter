using MediaProcessing.Common.Enums;
using MediaProcessing.Common.Results;
using MediaProcessing.Infrastructure.FileSystem;

namespace MediaProcessing.Feature.DeleteImage;

public class DeleteImageHandler(FileService fileService)
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

        return fileService.DeleteFile(folderPath, fileName);
    }
}
