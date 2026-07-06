using System.Text;
using Microsoft.Extensions.Options;
using Moq;
using WebpConverter.Common.Enums;
using WebpConverter.Common.Options;
using WebpConverter.Common.Results;
using WebpConverter.Feature.DeleteImage;
using WebpConverter.Feature.ProcessImage;
using WebpConverter.Infrastructure.Compression;
using WebpConverter.Infrastructure.FileSystem;
using Xunit;

namespace WebpConverter.Tests.UnitTests;

public class ProcessImageHandlerTests
{
    private static readonly byte[] ValidGifBytes = new byte[]
    {
        0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x01, 0x00, 0x01, 0x00,
        0x80, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x2C,
        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x02,
        0x02, 0x44, 0x01, 0x00, 0x3B
    };

    private readonly Mock<IWebPCompressor> _webPCompressorMock;
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly Mock<IDeleteImageHandler> _deleteImageHandlerMock;
    private readonly IOptions<AppOptions> _options;
    private readonly ProcessImageHandler _handler;

    public ProcessImageHandlerTests()
    {
        _webPCompressorMock = new Mock<IWebPCompressor>();
        _fileServiceMock = new Mock<IFileService>();
        _deleteImageHandlerMock = new Mock<IDeleteImageHandler>();

        var appOptions = new AppOptions
        {
            StoragePath = "/var/www/storage",
            PublicUrlPath = "https://cdn.example.com/images",
            MaxFileSizeBytes = 5 * 1024 * 1024,
            ImageCompressionQuality = 80
        };
        _options = Options.Create(appOptions);

        _handler = new ProcessImageHandler(
            _options,
            _webPCompressorMock.Object,
            _fileServiceMock.Object,
            _deleteImageHandlerMock.Object
        );
    }

    [Fact]
    public async Task HandleAsync_WithNullStream_ReturnsInvalidStreamError()
    {
        // Arrange
        var command = new ImageToProcess(null!, "test.jpg");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.InvalidStream, result.Error);
    }

    [Fact]
    public async Task HandleAsync_WithOversizedImage_ReturnsImageTooLargeError()
    {
        // Arrange
        var oversizedStream = new MemoryStream(new byte[6 * 1024 * 1024]); // 6MB
        var command = new ImageToProcess(oversizedStream, "huge.jpg");

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ImageTooLarge, result.Error);
    }

    [Fact]
    public async Task HandleAsync_WithValidImage_ProcessesAndReturnsPublicUrl()
    {
        // Arrange
        var validStream = new MemoryStream(ValidGifBytes);
        var command = new ImageToProcess(validStream, "photo.gif");

        // Mock FileService
        _fileServiceMock
            .Setup(x => x.GenerateFileName("photo"))
            .Returns("photo-12345");
            
        _fileServiceMock
            .Setup(x => x.SaveAsync(It.IsAny<Stream>(), It.IsAny<string>()))
            .ReturnsAsync(ResultMedia.Ok(Operation.Saved));

        // Act
        var result = await _handler.HandleAsync(command);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal("https://cdn.example.com/images/photo-12345.gif", result.Value);
        
        // GIF is passed through — compressor must NOT be called
        _webPCompressorMock.Verify(x => x.ConvertToWebpAsync(It.IsAny<Stream>(), It.IsAny<int>()), Times.Never);
        var expectedPath = Path.Combine("/var/www/storage", "photo-12345.gif");
        _fileServiceMock.Verify(x => x.SaveAsync(It.IsAny<Stream>(), expectedPath), Times.Once);
    }
}
