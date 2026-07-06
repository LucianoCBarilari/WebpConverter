using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace WebpConverter.Tests.IntegrationTests;

public class ImageProcessorIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    private static readonly byte[] ValidGifBytes = new byte[]
    {
        0x47, 0x49, 0x46, 0x38, 0x39, 0x61, 0x01, 0x00, 0x01, 0x00,
        0x80, 0x00, 0x00, 0xFF, 0xFF, 0xFF, 0x00, 0x00, 0x00, 0x2C,
        0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x01, 0x00, 0x00, 0x02,
        0x02, 0x44, 0x01, 0x00, 0x3B
    };

    public ImageProcessorIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Post_CompressImage_WithMissingFile_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var content = new MultipartFormDataContent();

        // Act - Send an empty multipart request (without the "File" IFormFile)
        var response = await client.PostAsync("/api/image-processor", content);

        // Assert - Should return 400 Bad Request due to Controller validation
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Post_CompressImage_WithCorruptImage_ReturnsBadRequest()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(new byte[] { 0x00, 0x01, 0x02 }); // Corrupted
        content.Add(fileContent, "File", "corrupt.jpg");

        // Act
        var response = await client.PostAsync("/api/image-processor", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var body = await response.Content.ReadAsStringAsync();
        Assert.Contains("Corrupted Image", body);
    }

    [Fact]
    public async Task Post_CompressImage_WithValidImage_ReturnsOk()
    {
        // Arrange
        var client = _factory.CreateClient();
        using var content = new MultipartFormDataContent();
        var fileContent = new ByteArrayContent(ValidGifBytes); 
        fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/gif");
        content.Add(fileContent, "File", "valid_image.gif");
        content.Add(new StringContent("valid_image"), "FileName");

        // Act
        var response = await client.PostAsync("/api/image-processor", content);

        // Assert
        var body = await response.Content.ReadAsStringAsync();
        Assert.True(response.IsSuccessStatusCode, $"Expected OK, but got {response.StatusCode}. Body: {body}");
        Assert.Contains(".gif", body); // GIFs are passed through without conversion
    }
}
