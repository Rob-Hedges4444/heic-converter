using ImageMagick;

namespace HeicConverter.Core;

/// <summary>HEIC/HEIF-to-JPEG conversion implemented with Magick.NET.</summary>
public sealed class HeicImageConverter : IImageConverter
{
    private static readonly HashSet<string> SupportedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".heic", ".heif"
    };

    public Task<string> ConvertToJpegAsync(string inputPath, string outputDirectory, int jpegQuality = 90,
        CancellationToken cancellationToken = default) => Task.Run(() =>
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(inputPath);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputDirectory);

        if (!File.Exists(inputPath))
            throw new FileNotFoundException("The selected file no longer exists.", inputPath);
        if (!SupportedExtensions.Contains(Path.GetExtension(inputPath)))
            throw new NotSupportedException("Select a HEIC or HEIF image.");
        if (jpegQuality is < 1 or > 100)
            throw new ArgumentOutOfRangeException(nameof(jpegQuality), "JPEG quality must be from 1 to 100.");

        cancellationToken.ThrowIfCancellationRequested();
        Directory.CreateDirectory(outputDirectory);
        var outputPath = GetAvailablePath(outputDirectory, Path.GetFileNameWithoutExtension(inputPath));

        using var image = new MagickImage(inputPath);
        cancellationToken.ThrowIfCancellationRequested();
        image.AutoOrient();
        image.BackgroundColor = MagickColors.White;
        image.Alpha(AlphaOption.Remove);
        image.Format = MagickFormat.Jpeg;
        image.Quality = (uint)jpegQuality;
        image.Strip(); // Removes camera metadata, including location data.
        image.Write(outputPath);
        return outputPath;
    }, cancellationToken);

    private static string GetAvailablePath(string directory, string baseName)
    {
        var candidate = Path.Combine(directory, baseName + ".jpg");
        for (var suffix = 1; File.Exists(candidate); suffix++)
            candidate = Path.Combine(directory, $"{baseName} ({suffix}).jpg");
        return candidate;
    }
}
