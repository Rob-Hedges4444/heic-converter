# HEIC to JPEG Converter

A Windows desktop MVP for internal users. It supports selecting or dragging in multiple `.heic` / `.heif` files and converts them to JPEG. The demonstration configuration saves output to `Desktop\HEIC Conversions`; a production deployment must replace this with an IT-managed directory. Existing images are never overwritten: a number is added to duplicate names.

## Build and run

Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) on a Windows development machine, then run from this folder:

```powershell
dotnet restore
dotnet build -c Release
dotnet run --project .\src\HeicConverter.Desktop
```

The default output is `Desktop\HEIC Conversions`. To override it locally, copy `src\HeicConverter.Desktop\appsettings.local.example.json` to `appsettings.local.json` in the same folder and replace `YourName` with your Windows user name. This local file is intentionally ignored by Git and is never published. Production deployments should configure `appsettings.json` through the approved deployment process instead.

To create a self-contained Windows executable for distribution:

```powershell
dotnet publish .\src\HeicConverter.Desktop -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o .\publish
```

Distribute the contents of `publish` (not just the EXE if Windows produces supporting files). Test the publish on a standard user machine before deployment.

## Design notes

- `HeicConverter.Core` holds the reusable conversion service for a future claim-system integration.
- `HeicConverter.Desktop` is the WPF user interface.
- The converter auto-rotates phone images, writes JPEGs at a user-selected quality, and strips source metadata (including location data). Retaining metadata can be added only after a privacy/business review.
- Image conversion is provided by Magick.NET. Before enterprise deployment, record the package version and review its current licence/third-party notices with your normal software-approval process.
- Conversion is deliberately disabled until IT configures `ManagedOutputDirectory` in the deployed `appsettings.json`. See `docs/REGULATED_DEPLOYMENT_CHECKLIST.md`.

## Regulated use

Read `SECURITY.md` and complete `docs/REGULATED_DEPLOYMENT_CHECKLIST.md` before processing live data. This code supports a controlled deployment; it does not make the firm ISO 27001 or FCA compliant by itself.

## Future claims-system use

Reference `HeicConverter.Core` and call:

```csharp
var converter = new HeicImageConverter();
var jpegPath = await converter.ConvertToJpegAsync(sourceHeicPath, destinationFolder);
```

For a process that cannot use a .NET assembly directly, put a thin, versioned integration layer around this public API instead of duplicating the image-conversion code.

## Licence

Licensed under the Apache License, Version 2.0. See `LICENSE`.
