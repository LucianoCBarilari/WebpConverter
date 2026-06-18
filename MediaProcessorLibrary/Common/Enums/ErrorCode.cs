namespace MediaProcessorLibrary.Common.Enums;

public enum ErrorCode
{
    
    Unexpected,
    InvalidStream,
    Unauthorized,
    IOError,

    
    ImageTooLarge,
    InvalidDimensions,
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