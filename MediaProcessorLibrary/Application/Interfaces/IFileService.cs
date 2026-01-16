using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IFileService
    {
        public Dictionary<string, string> ListFiles(string folderPath);
        public Dictionary<string, string> ListFiles(string folderPath, string searchPattern);
        public bool FileExist(string folderPath, string fileName);
        public Result CreateFile(string folderPath, string fileName, string fileExtension);
        public Result DeleteFile(string folderPath, string fileName);
        Task<Result> SaveAsync(Stream content, string fullPath);
    }
}
