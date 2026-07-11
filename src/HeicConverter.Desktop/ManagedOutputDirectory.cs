using System.Text.Json;
using System.IO;

namespace HeicConverter.Desktop;

/// <summary>Reads the organisation-controlled output location supplied at deployment.</summary>
internal static class ManagedOutputDirectory
{
    private static readonly string[] SettingsFileNames = ["appsettings.local.json", "appsettings.json"];

    public static bool TryGet(out string directory, out string error)
    {
        directory = string.Empty;
        try
        {
            foreach (var settingsFileName in SettingsFileNames)
            {
                var settingsPath = Path.Combine(AppContext.BaseDirectory, settingsFileName);
                if (!File.Exists(settingsPath)) continue;

                using var document = JsonDocument.Parse(File.ReadAllText(settingsPath));
                if (!document.RootElement.TryGetProperty("ManagedOutputDirectory", out var property) ||
                    string.IsNullOrWhiteSpace(property.GetString())) continue;

                directory = Path.GetFullPath(property.GetString()!);
                Directory.CreateDirectory(directory);
                error = string.Empty;
                return true;
            }

            error = "IT has not configured an approved output location.";
            return false;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or JsonException or ArgumentException or NotSupportedException)
        {
            error = $"The approved output location cannot be used: {ex.Message}";
            return false;
        }
    }
}
