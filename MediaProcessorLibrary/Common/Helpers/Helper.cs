using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MediaProcessorLibrary.Common.Helpers
{
    public class Helper
    {
        public string GenerateFileName(string baseName)
        {
            if (string.IsNullOrWhiteSpace(baseName))
                return string.Empty;

            Regex regex = new Regex(@"[^a-zA-Z0-9_]");

            if (regex.IsMatch(baseName))
                return string.Empty;

            return $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}";
        }
    }
}
