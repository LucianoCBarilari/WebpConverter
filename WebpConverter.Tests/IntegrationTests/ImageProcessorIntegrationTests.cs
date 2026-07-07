using System.Net;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Google.Protobuf;
using WebpConverter;

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

    private ImageProcessor.ImageProcessorClient CreateClient()
    {
        var handler = _factory.Server.CreateHandler();
        var channel = GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
        {
            HttpHandler = handler
        });
        return new ImageProcessor.ImageProcessorClient(channel);
    }

    [Fact]
    public async Task ConvertImage_WithEmptyData_ReturnsError()
    {
        // Arrange
        var client = CreateClient();
        var request = new ConvertImageRequest
        {
            FileName = "empty.jpg",
            ImageData = ByteString.Empty
        };

        // Act
        var response = await client.ConvertImageAsync(request);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("InvalidStream", response.ErrorCode);
    }

    [Fact]
    public async Task ConvertImage_WithCorruptImage_ReturnsError()
    {
        // Arrange
        var client = CreateClient();
        var request = new ConvertImageRequest
        {
            FileName = "corrupt.jpg",
            ImageData = ByteString.CopyFrom(new byte[] { 0x00, 0x01, 0x02 })
        };

        // Act
        var response = await client.ConvertImageAsync(request);

        // Assert
        Assert.False(response.Success);
        Assert.Equal("CorruptedImage", response.ErrorCode);
    }

    [Fact]
    public async Task ConvertImage_WithValidImage_ReturnsSuccess()
    {
        // Arrange
        var client = CreateClient();
        var request = new ConvertImageRequest
        {
            FileName = "valid_image.gif",
            ImageData = ByteString.CopyFrom(ValidGifBytes),
            Subfolder = "test-app"
        };

        // Act
        var response = await client.ConvertImageAsync(request);

        // Assert
        Assert.True(response.Success);
        Assert.Contains(".gif", response.PublicPath);
        Assert.True(string.IsNullOrEmpty(response.ErrorCode));
    }
}
