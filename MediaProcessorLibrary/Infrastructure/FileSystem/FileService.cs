using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Infrastructure.FileSystem
{
    public class FileService : IFileService
    {

        /// <summary>
        /// Lists all files in the specified folder.
        /// </summary>
        /// <param name="folderPath">The full path to the folder.</param>
        /// <returns>A dictionary where keys are file names and values are their full paths.</returns>
        public Dictionary<string, string> ListFiles(string folderPath)
        {
            return ListFiles(folderPath, null);
        }
        /// <summary>
        /// Lists files in the specified folder, optionally filtering by a search pattern.
        /// </summary>
        /// <param name="folderPath">The full path to the folder.</param>
        /// <param name="searchPattern">The search string to match against the names of files. This parameter can contain a combination of valid literal path and wildcard (* and ?) characters.</param>
        /// <returns>A dictionary where keys are file names and values are their full paths.</returns>
        public Dictionary<string, string> ListFiles(string folderPath, string searchPattern)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return new();

            if (!Directory.Exists(folderPath))
                return new();

            var files = string.IsNullOrWhiteSpace(searchPattern)
                ? Directory.GetFiles(folderPath)
                : Directory.GetFiles(folderPath, searchPattern);

            return files.ToDictionary(
                path => Path.GetFileName(path),
                path => path
            );
        }
        /// <summary>
        /// Checks if a file exists.
        /// </summary>
        /// <param name="folderPath">The full path to the folder.</param>
        /// <param name="fileName">The name of the file to check.</param>
        /// <returns>True if the file exists, false otherwise.</returns>
        public bool FileExist(string folderPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderPath) || string.IsNullOrWhiteSpace(fileName))
                return false;

            string filePath = Path.Combine(folderPath, fileName);
            return File.Exists(filePath);
        }
        /// <summary>
        /// Creates a new file.
        /// </summary>
        /// <param name="folderPath">The folder path where the file will be created.</param>
        /// <param name="fileName">The name of the file.</param>
        /// <param name="fileExtension">The file extension (e.g., ".txt", ".jpg").</param>
        /// <returns>Result of the file creation operation.</returns>
        public FileResult CreateFile(string folderPath, string fileName, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return FileResult.PathEmpty;

            if (string.IsNullOrWhiteSpace(fileName))
                return FileResult.FileNameEmpty;

            string fullPath = Path.Combine(folderPath, $"{fileName}{fileExtension}");

            if (File.Exists(fullPath))
                return FileResult.FileExist;

            File.Create(fullPath).Dispose(); 
            return FileResult.Created;
        }

        /// <summary>
        /// Deletes a specified file.
        /// </summary>
        /// <param name="folderPath">The path to the folder containing the file.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A <see cref="FileResult"/> indicating the outcome of the delete operation.</returns>
        public FileResult DeleteFile(string folderPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return FileResult.PathEmpty;

            if (string.IsNullOrWhiteSpace(fileName))
                return FileResult.FileNameEmpty;

            string fullPath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullPath))
                return FileResult.NotFound;

            File.Delete(fullPath);
            return FileResult.Deleted;
        }
        public async Task<FileResult> SaveAsync(Stream content, string fullPath)
        {
            if (content == null || string.IsNullOrWhiteSpace(fullPath))
                return FileResult.NotFound;

            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (content.CanSeek)
                    content.Position = 0;

                using var fileStream = File.Create(fullPath);
                await content.CopyToAsync(fileStream);

                return FileResult.Created;
            }
            catch
            {
                return FileResult.Error;
            }
        }

    }
}
