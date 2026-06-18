using MediaProcessorLibrary.Common.Enums;
using MediaProcessorLibrary.Common.Results;

namespace MediaProcessorLibrary.Infrastructure.FileSystem;

public class DirectoryService 
{
    /// <summary>
    /// Lists subfolders within a specified folder, returning a dictionary where keys are folder names and values are their full paths.
    /// </summary>
    /// <param name="folderName">The name of the folder to list subfolders from.</param>
    /// <returns>A dictionary of subfolder names and their paths, or an empty dictionary if the folder does not exist or the name is invalid.</returns>
    public Dictionary<string, string> ListFolders(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return new();

        string currentPath = GetCurrentFolderPath(folderName);

        if (!Directory.Exists(currentPath))
            return new();

        return Directory.GetDirectories(currentPath)
            .ToDictionary(
                path => Path.GetFileName(path),
                path => path
            );
    }
    /// <summary>
    /// Creates a new folder at the specified location with the given name.
    /// </summary>
    /// <param name="folderLocation">The path where the new folder should be created.</param>
    /// <param name="folderName">The name of the new folder.</param>
    /// <returns>A <see cref="FolderResult"/> indicating the outcome of the operation.</returns>
    public ResultMedia CreateFolder(string folderLocation, string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderLocation))
            return ResultMedia.Fail(ErrorCode.PathEmpty);

        if (string.IsNullOrWhiteSpace(folderName))
            return ResultMedia.Fail(ErrorCode.FolderNameEmpty);

        string newFolderPath = Path.Combine(folderLocation, folderName);

        if (Directory.Exists(newFolderPath))
            return ResultMedia.Fail(ErrorCode.AlreadyExists);

        try
        {
            Directory.CreateDirectory(newFolderPath);
            return ResultMedia.Ok(Operation.Created);
        }
        catch (UnauthorizedAccessException)
        {
            return ResultMedia.Fail(ErrorCode.Unauthorized);
        }
        catch (IOException)
        {
            return ResultMedia.Fail(ErrorCode.IOError);
        }
        catch
        {
            return ResultMedia.Fail(ErrorCode.Unexpected);
        }
    }
    /// <summary>
    /// Gets the full path for a folder relative to the current working directory.
    /// </summary>
    /// <param name="folderName">The name of the folder.</param>
    /// <returns>The full, combined path of the current directory and the specified folder name.</returns>
    public string GetCurrentFolderPath(string folderName)
    {
        return Path.Combine(Directory.GetCurrentDirectory(), folderName);
    }
    /// <summary>
    /// Checks if a folder exists.
    /// </summary>
    /// <param name="folderName">The name of the folder to check for existence.</param>
    /// <returns>True if the folder exists, false otherwise.</returns>
    public bool FolderExist(string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderName))
            return false;

        return Directory.Exists(GetCurrentFolderPath(folderName));
    }
    /// <summary>
    /// Deletes a specified folder.
    /// </summary>
    /// <param name="folderLocation">The path where the folder to be deleted is located.</param>
    /// <param name="folderName">The name of the folder to delete.</param>
    /// <returns>A <see cref="FolderResult"/> indicating the outcome of the delete operation.</returns>
    public ResultMedia DeleteFolder(string folderLocation, string folderName)
    {
        if (string.IsNullOrWhiteSpace(folderLocation))
            return ResultMedia.Fail(ErrorCode.PathEmpty);

        if (string.IsNullOrWhiteSpace(folderName))
            return ResultMedia.Fail(ErrorCode.FolderNameEmpty);

        string folderPath = Path.Combine(folderLocation, folderName);

        if (!Directory.Exists(folderPath))
            return ResultMedia.Fail(ErrorCode.NotFound);

        try
        {
            Directory.Delete(folderPath, true);
            return ResultMedia.Ok(Operation.Deleted);
        }
        catch (UnauthorizedAccessException)
        {
            return ResultMedia.Fail(ErrorCode.Unauthorized);
        }
        catch (IOException)
        {
            return ResultMedia.Fail(ErrorCode.IOError);
        }
        catch
        {
            return ResultMedia.Fail(ErrorCode.Unexpected);
        }
    }
}
