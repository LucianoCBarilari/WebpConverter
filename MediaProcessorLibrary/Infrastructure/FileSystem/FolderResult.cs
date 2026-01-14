using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Infrastructure.FileSystem
{
    public enum FolderResult
    {
            Created,
            Deleted,
            FolderExist,
            PathEmpty, 
            FolderNameEmpty,
            NotFound
    }
}
