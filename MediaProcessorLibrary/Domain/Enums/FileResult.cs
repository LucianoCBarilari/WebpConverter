using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Domain.Enums
{
    public enum FileResult
    {
        Created,
        Deleted,
        FileExist,
        PathEmpty,
        FileNameEmpty,
        NotFound
    }
}
