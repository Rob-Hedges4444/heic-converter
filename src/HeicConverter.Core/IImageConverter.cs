namespace HeicConverter.Core;

/// <summary>Converts a supported image file to JPEG.</summary>
public interface IImageConverter
{
    Task<string> ConvertToJpegAsync(string inputPath, string outputDirectory, int jpegQuality = 90,
        CancellationToken cancellationToken = default);
}
