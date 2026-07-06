using Microsoft.Extensions.Options;

namespace WebpConverter.Common.Options;

public class AppOptionsValidator : IValidateOptions<AppOptions>
{
    public ValidateOptionsResult Validate(string? name, AppOptions options)
    {
        var errors = new List<string>();

        // StoragePath
        if (string.IsNullOrWhiteSpace(options.StoragePath))
        {
            errors.Add("StoragePath cannot be empty.");
        }
        else if (options.StoragePath.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
        {
            errors.Add("StoragePath contains invalid characters.");
        }

        // PublicUrlPath
        if (string.IsNullOrWhiteSpace(options.PublicUrlPath))
        {
            errors.Add("PublicUrlPath cannot be empty.");
        }
        else if (!Uri.IsWellFormedUriString(options.PublicUrlPath, UriKind.RelativeOrAbsolute))
        {
            errors.Add("PublicUrlPath is not a valid URL or path.");
        }

        // ImageCompressionQuality
        if (options.ImageCompressionQuality < 1 || options.ImageCompressionQuality > 100)
        {
            errors.Add("ImageCompressionQuality must be between 1 and 100.");
        }

        // MaxFileSizeBytes
        if (options.MaxFileSizeBytes <= 0)
        {
            errors.Add("MaxFileSizeBytes must be greater than 0.");
        }

        return errors.Count > 0
            ? ValidateOptionsResult.Fail(errors)
            : ValidateOptionsResult.Success;
    }
}