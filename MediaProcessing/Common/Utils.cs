using System.Text.RegularExpressions;

namespace MediaProcessing.Common;

public static class Utils 
{
    public static string GenerateFileName(string baseName)
    {
        if (string.IsNullOrWhiteSpace(baseName))
            return string.Empty;

        Regex regex = new(@"[^a-zA-Z0-9_]");

        if (regex.IsMatch(baseName))
            return string.Empty;

        return $"{baseName}_{DateTime.Now:yyyyMMdd_HHmmss}";
    }
}
