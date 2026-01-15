using MediaProcessorLibrary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.Interfaces
{
    public interface IImageValidator
    {
        ImageValidationResult Validate(
            Stream imageStream,
            int maxWidth,
            int maxHeight,
            long maxSizeBytes
        );
    }

}
