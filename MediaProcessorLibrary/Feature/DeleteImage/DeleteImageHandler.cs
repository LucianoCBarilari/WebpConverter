using MediaProcessorLibrary.Common.Enums;
using MediaProcessorLibrary.Common.Results;
using MediaProcessorLibrary.Infrastructure.FileSystem;

namespace MediaProcessorLibrary.Feature.DeleteImage;

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
