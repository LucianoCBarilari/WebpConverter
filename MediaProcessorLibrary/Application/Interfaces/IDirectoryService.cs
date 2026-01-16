using MediaProcessorLibrary.Application.Results;
using MediaProcessorLibrary.Domain.Enums;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IDirectoryService
    {
        public Result CreateFolder(string folderLocation, string folderName);
        public Result DeleteFolder(string folderLocation, string folderName);
        public Dictionary<string, string> ListFolders(string folderName);
        public string GetCurrentFolderPath(string folderName);
        public bool FolderExist(string folderName);
        
    }
}
