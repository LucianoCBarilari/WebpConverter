using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.Results
{
    public enum ErrorCode
    {
        PathEmpty,
        NameEmpty,
        InvalidStream,
        AlreadyExists,
        NotFound,
        InvalidImage,
        ImageTooLarge,
        CorruptedImage,
        FolderNameEmpty,
        Unauthorized,
        IOError,
        Unexpected
    }
}
