using Moq;
using WebpConverter.Common.Enums;
using WebpConverter.Common.Results;
using WebpConverter.Feature.DeleteImage;
using WebpConverter.Infrastructure.FileSystem;
using Xunit;

namespace WebpConverter.Tests.UnitTests;

public class DeleteImageHandlerTests
{
    private readonly Mock<IFileService> _fileServiceMock;
    private readonly DeleteImageHandler _handler;

    public DeleteImageHandlerTests()
    {
        _fileServiceMock = new Mock<IFileService>();
        _handler = new DeleteImageHandler(_fileServiceMock.Object);
    }

    [Fact]
    public void Delete_WithEmptyFolderPath_ReturnsPathEmptyError()
    {
        // Act
        var result = _handler.Delete("", "image.webp");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.PathEmpty, result.Error);
    }

    [Fact]
    public void Delete_WithEmptyFileName_ReturnsFileNameEmptyError()
    {
        // Act
        var result = _handler.Delete("/var/www/images", "   ");

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Equal(ErrorCode.FileNameEmpty, result.Error);
    }

    [Fact]
    public void Delete_WithValidData_ReturnsSuccess()
    {
        // Arrange
        _fileServiceMock
            .Setup(x => x.DeleteFile(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(ResultMedia.Ok(Operation.Deleted));

        // Act
        var result = _handler.Delete("/var/www/images", "valid_image.webp");

        // Assert
        Assert.True(result.IsSuccess);
        _fileServiceMock.Verify(x => x.DeleteFile("/var/www/images", "valid_image.webp"), Times.Once);
    }
}
