using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Domain.Enums
{
    public enum FolderResult
    {
            Created,
            Deleted,
            FolderExist,
            PathEmpty, 
            FolderNameEmpty,
            NotFound,
            Error
    }
}
