using WebpConverter.Common.Enums;

namespace WebpConverter.Common.Results;

public static class MediaErrors
{
    public static string GetMessage(this ErrorCode code) => code switch
    {
        ErrorCode.InvalidStream => "The provided image stream is null or empty.",
        ErrorCode.ImageTooLarge => "The image exceeds the maximum allowed file size.",
        ErrorCode.CorruptedImage => "The image data appears to be corrupted or invalid.",
        ErrorCode.InvalidImage => "The file provided is not a supported image format.",


        ErrorCode.PathEmpty => "The destination path was not provided.",
        ErrorCode.FileNameEmpty => "The output file name is missing.",
        ErrorCode.FolderNameEmpty => "The directory name cannot be empty.",
        ErrorCode.NotFound => "The specified file or directory was not found.",
        ErrorCode.AlreadyExists => "A file with the same name already exists in the destination.",


        ErrorCode.Unauthorized => "The application does not have permission to access the storage.",
        ErrorCode.IOError => "An error occurred during the disk I/O operation.",
        ErrorCode.SaveFailed => "Failed to save the processed image to the storage.",

        _ => "An unexpected error occurred during processing."
    };
}
