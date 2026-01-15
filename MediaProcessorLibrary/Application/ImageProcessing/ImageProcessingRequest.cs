using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.ImageProcessing
{
    public class ImageProcessingRequest
    {
        public Stream ImageStream { get; set; } 

        public int MaxWidth { get; set; }
        public int MaxHeight { get; set; }
        public long MaxSizeBytes { get; set; }

        public string OutputDirectory { get; set; } = string.Empty;
        public string OutputFileName { get; set; } = string.Empty;

        public int Quality { get; init; }
    }

}
