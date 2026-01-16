using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Domain.Enums
{
    public enum ImageValidationResult
    {
        Valid,
        Empty,
        Corrupted,
        InvalidDimensions,
        TooLarge,
        Error
    }
}
