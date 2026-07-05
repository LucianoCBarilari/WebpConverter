namespace WebpConverter.Common.Options;

public class AppOptions
{
    public const string SectionName = "AppOptions";
    public string StoragePath { get; set; } = string.Empty;
    public string PublicUrlPath { get; set; } = string.Empty;
    public int ImageCompressionQuality { get; set; }
    public long MaxFileSizeBytes { get; set; }
}
