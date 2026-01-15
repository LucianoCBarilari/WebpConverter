using System.Text.RegularExpressions;

namespace MediaProcessorLibrary.Common.Helpers
{
    public class Helper : IHelpers
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
