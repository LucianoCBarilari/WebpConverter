using WebpConverter.Common.Enums;
using WebpConverter.Feature.ProcessImage;
using Xunit;

namespace WebpConverter.Tests.UnitTests;

public class ImageValidatorTests
{
    [Fact]
    public void ValidateSize_WithStreamExceedingMax_ReturnsImageTooLarge()
    {
        // Arrange
        var stream = new MemoryStream(new byte[6 * 1024 * 1024]); // 6MB
        var maxSize = 5 * 1024 * 1024; // 5MB

        // Act
        var result = ImageValidator.ValidateSize(stream, maxSize);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.ImageTooLarge, result.Error);
    }

    [Fact]
    public void ValidateSize_WithStreamUnderMax_ReturnsSuccess()
    {
        // Arrange
        var stream = new MemoryStream(new byte[1 * 1024 * 1024]); // 1MB
        var maxSize = 5 * 1024 * 1024; // 5MB

        // Act
        var result = ImageValidator.ValidateSize(stream, maxSize);

        // Assert
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void ValidateImage_WithCorruptedData_ReturnsInvalidImage()
    {
        // Arrange
        var corruptedStream = new MemoryStream(new byte[] { 0x00, 0xFF, 0x00, 0x11 });

        // Act
        var result = ImageValidator.ValidateImage(corruptedStream);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.CorruptedImage, result.Error);
    }
}
