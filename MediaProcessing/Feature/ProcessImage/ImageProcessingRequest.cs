using System.ComponentModel.DataAnnotations;

namespace MediaProcessing.Feature.ProcessImage;
public class ImageProcessingRequest
{
    [Required]
    public Stream ImageStream { get; set; } = Stream.Null;
    [Range(1, 10000)]
    public int MaxWidth { get; set; }
    [Range(1, 10000)]
    public int MaxHeight { get; set; }
    [Range(1, 104857600)]
    public long MaxSizeBytes { get; set; }
    [Required]
    public string OutputDirectory { get; set; } = string.Empty;
    [Required]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessage = "Nombre de archivo inválido")]
    public string OutputFileName { get; set; } = string.Empty;
    [Range(1, 100)]
    public int Quality { get; init; } = 80;
}
