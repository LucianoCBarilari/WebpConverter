using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.Results;
using MediaProcessorLibrary.Domain.Enums;

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
        public Result CreateFile(string folderPath, string fileName, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return Result.Fail(ErrorCode.PathEmpty);

            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Fail(ErrorCode.FolderNameEmpty);

            string fullPath = Path.Combine(folderPath, $"{fileName}{fileExtension}");

            if (File.Exists(fullPath))
                return Result.Fail(ErrorCode.AlreadyExists);

            try
            {
                using var _ = File.Create(fullPath);
                return Result.Ok(Operation.Created);
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail(ErrorCode.Unauthorized);
            }
            catch (IOException)
            {
                return Result.Fail(ErrorCode.IOError);
            }
        }


        /// <summary>
        /// Deletes a specified file.
        /// </summary>
        /// <param name="folderPath">The path to the folder containing the file.</param>
        /// <param name="fileName">The name of the file to delete.</param>
        /// <returns>A <see cref="FileResult"/> indicating the outcome of the delete operation.</returns>
        public Result DeleteFile(string folderPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return Result.Fail(ErrorCode.PathEmpty);

            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Fail(ErrorCode.NameEmpty);

            string fullPath = Path.Combine(folderPath, fileName);

            if (!File.Exists(fullPath))
                return Result.Fail(ErrorCode.NotFound);

            try
            {
                File.Delete(fullPath);
                return Result.Ok(Operation.Deleted);
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail(ErrorCode.Unauthorized);
            }
            catch (IOException)
            {
                return Result.Fail(ErrorCode.IOError);
            }
        }

        /// <summary>
        /// Asynchronously saves a stream's content to a specified file path.
        /// </summary>
        /// <param name="content">The stream to save.</param>
        /// <param name="fullPath">The full path where the file will be saved.</param>
        /// <returns>A <see cref="Result"/> indicating the outcome of the save operation.</returns>
        public async Task<Result> SaveAsync(Stream content, string fullPath)
        {
            if (content == null)
                return Result.Fail(ErrorCode.InvalidStream);

            if (string.IsNullOrWhiteSpace(fullPath))
                return Result.Fail(ErrorCode.PathEmpty);

            try
            {
                var directory = Path.GetDirectoryName(fullPath);
                if (!Directory.Exists(directory))
                    Directory.CreateDirectory(directory);

                if (content.CanSeek)
                    content.Position = 0;

                using var fileStream = File.Create(fullPath);
                await content.CopyToAsync(fileStream);

                return Result.Ok(Operation.Saved);
            }
            catch (UnauthorizedAccessException)
            {
                return Result.Fail(ErrorCode.Unauthorized);
            }
            catch (IOException)
            {
                return Result.Fail(ErrorCode.IOError);
            }
            catch
            {
                return Result.Fail(ErrorCode.Unexpected);
            }
        }
    }
}
