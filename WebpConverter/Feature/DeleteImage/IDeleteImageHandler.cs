using WebpConverter.Common.Results;

namespace WebpConverter.Feature.DeleteImage;

public interface IDeleteImageHandler
{
    ResultMedia Delete(string folderPath, string fileName);
}
