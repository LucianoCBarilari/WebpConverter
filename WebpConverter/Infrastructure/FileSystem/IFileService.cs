using WebpConverter.Common.Results;

namespace WebpConverter.Infrastructure.FileSystem;

public interface IFileService
{
    Dictionary<string, string> ListFiles(string folderPath);
    Dictionary<string, string> ListFiles(string folderPath, string searchPattern);
    bool FileExist(string folderPath, string fileName);
    ResultMedia CreateFile(string folderPath, string fileName, string fileExtension);
    ResultMedia DeleteFile(string folderPath, string fileName);
    Task<ResultMedia> SaveAsync(Stream content, string fullPath);
    string GenerateFileName(string baseName);
}
