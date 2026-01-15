using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.ImageProcessing
{
    public enum ImageProcessingResult
    {
        Success,
        InvalidInput,
        InvalidImage,
        ValidationFailed,
        CompressionFailed,
        SaveFailed
    }
}
