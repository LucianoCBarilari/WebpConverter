namespace MediaProcessorLibrary.Feature.ProcessImage
{
    public class ImageProcessRequest
    {
        public IFormFile File { get; set; } = default!;
        public string? FileName { get; set; }
        public int? MaxFileSize { get; set; }
        public Guid ImageSizeId { get; set; }
        public int? Quality { get; set; }
    }
}
