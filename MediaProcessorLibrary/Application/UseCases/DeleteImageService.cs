using MediaProcessorLibrary.Application.Interfaces;
using MediaProcessorLibrary.Application.Results;

namespace MediaProcessorLibrary.Application.UseCases
{
    public class DeleteImageService : IDeleteImageService
    {
        private readonly IFileService _fileService;
        public DeleteImageService(IFileService fileService)
        {
            _fileService = fileService;
        }
        public Result Delete(string folderPath, string fileName)
        {
            if (string.IsNullOrWhiteSpace(folderPath))
                return Result.Fail(ErrorCode.PathEmpty);

            if (string.IsNullOrWhiteSpace(fileName))
                return Result.Fail(ErrorCode.NameEmpty);

            return _fileService.DeleteFile(folderPath, fileName);
        }
    }
}
