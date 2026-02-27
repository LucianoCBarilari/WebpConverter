namespace MediaProcessorLibrary;

public class ImageProcessingService(
        FileService fileService,
        DirectoryService directoryService,
        ImageValidator imageValidator,
        WebPCompressor webPCompressor,
        Utils Utility) :IImageProcessingService
{
    /// <summary>
    /// Processes an image: validates, compresses to WebP, and saves it to the specified directory.
    /// </summary>
    /// <param name="request">The object containing the image stream and processing parameters.</param>
    /// <returns>A ResultMedia containing the final file path if successful, or an error code.</returns>
    public async Task<ResultMedia<string>> ImageProcessAsync(ImageProcessingRequest request)
    {
        if (request?.ImageStream == null)
            return ResultMedia<string>.Fail(ErrorCode.InvalidStream);

        var validation = imageValidator.Validate(
            request.ImageStream,
            request.MaxWidth,
            request.MaxHeight,
            request.MaxSizeBytes);

        if (!validation.IsSuccess)
            return ResultMedia<string>.Fail(validation.Error ?? ErrorCode.InvalidImage);

        if (request.ImageStream.CanSeek)
            request.ImageStream.Position = 0;

        using var compressedStream = await webPCompressor.ConvertToWebpAsync(
            request.ImageStream,
            request.Quality);

        directoryService.FolderExist(request.OutputDirectory);

        var generatedName = Utility.GenerateFileName(request.OutputFileName);

        if (string.IsNullOrEmpty(generatedName))
            return ResultMedia<string>.Fail(ErrorCode.FileNameEmpty);

        var fullPath = Path.Combine(request.OutputDirectory, $"{generatedName}.webp");

        var saveResult = await fileService.SaveAsync(compressedStream, fullPath);

        if (!saveResult.IsSuccess)
            return ResultMedia<string>.Fail(saveResult.Error ?? ErrorCode.SaveFailed);

        return ResultMedia<string>.Ok(fullPath, Operation.Saved);
    }
    /// <summary>
    /// Deletes an image file using the Result Pattern.
    /// </summary>
    public ResultMedia Delete(string folderPath, string fileName)
    {
      
        if (string.IsNullOrWhiteSpace(folderPath))
            return ResultMedia.Fail(ErrorCode.PathEmpty);

        if (string.IsNullOrWhiteSpace(fileName))
            return ResultMedia.Fail(ErrorCode.FileNameEmpty);

        return fileService.DeleteFile(folderPath, fileName);
    }
}