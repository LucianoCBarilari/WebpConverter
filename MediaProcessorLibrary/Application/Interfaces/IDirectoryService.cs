using MediaProcessorLibrary.Domain.Enums;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IDirectoryService
    {
        public FolderResult CreateFolder(string folderLocation, string folderName);
        public FolderResult DeleteFolder(string folderLocation, string folderName);
        public Dictionary<string, string> ListFolders(string folderName);
        public string GetCurrentFolderPath(string folderName);
        public bool FolderExist(string folderName);
        
    }
}
