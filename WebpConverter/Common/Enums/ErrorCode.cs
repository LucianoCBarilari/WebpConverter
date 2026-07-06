namespace WebpConverter.Common.Enums;

public enum ErrorCode
{

    Unexpected,
    InvalidStream,
    Unauthorized,
    IOError,


    ImageTooLarge,
    CorruptedImage,
    InvalidImage,


    PathEmpty,
    FileNameEmpty,
    FolderNameEmpty,
    NotFound,
    AlreadyExists,


    ValidationFailed,
    CompressionFailed,
    SaveFailed
}