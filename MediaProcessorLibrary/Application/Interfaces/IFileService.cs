using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IFileService
    {
        Dictionary<string, string> ListFiles(string folderPath);
        Dictionary<string, string> ListFiles(string folderPath, string searchPattern);
        bool FileExist(string folderPath, string fileName);
        Result CreateFile(string folderPath, string fileName, string fileExtension);
        Result DeleteFile(string folderPath, string fileName);
        Task<Result> SaveAsync(Stream content, string fullPath);
    }
}
