using WebpConverter.Infrastructure.FileSystem;
using Xunit;

namespace WebpConverter.Tests.UnitTests;

public class FileServiceTests
{
    private readonly FileService _fileService;

    public FileServiceTests()
    {
        _fileService = new FileService();
    }

    [Fact]
    public void GenerateFileName_WithValidBaseName_ReturnsFormattedName()
    {
        // Arrange
        var baseName = "my Image name! @#$";

        // Act
        var result = _fileService.GenerateFileName(baseName);

        // Assert
        Assert.NotNull(result);
        Assert.DoesNotContain(" ", result); // Should be regex replaced
        Assert.DoesNotContain("!", result);
        Assert.Contains("my-Image-name", result); // Cleaned version
        Assert.True(result.Length > baseName.Length); // Due to datetime and guid
    }

    [Fact]
    public void GenerateFileName_WithEmptyBaseName_ReturnsEmptyString()
    {
        // Arrange
        var baseName = "";

        // Act
        var result = _fileService.GenerateFileName(baseName);

        // Assert
        Assert.Equal(string.Empty, result);
    }
}
