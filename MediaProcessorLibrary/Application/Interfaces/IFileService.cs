using MediaProcessorLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IFileService
    {
        public Dictionary<string, string> ListFiles(string folderPath);
        public Dictionary<string, string> ListFiles(string folderPath, string searchPattern);
        public bool FileExist(string folderPath, string fileName);
        public FileResult CreateFile(string folderPath, string fileName, string fileExtension);
        public FileResult DeleteFile(string folderPath, string fileName);
        Task<FileResult> SaveAsync(Stream content, string fullPath);
    }
}
