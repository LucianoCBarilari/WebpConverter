using MediaProcessorLibrary.Application.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Application.UseCases
{
    public interface IDeleteImageService
    {
        Result Delete(string folderPath, string fileName);
    }
}
