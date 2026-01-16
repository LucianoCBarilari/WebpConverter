using MediaProcessorLibrary.Application.Results;
using MediaProcessorLibrary.Domain.Enums;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IDirectoryService
    {
        Result CreateFolder(string folderLocation, string folderName);
        Result DeleteFolder(string folderLocation, string folderName);
        Dictionary<string, string> ListFolders(string folderName);
        string GetCurrentFolderPath(string folderName);
        bool FolderExist(string folderName);
        
    }
}
