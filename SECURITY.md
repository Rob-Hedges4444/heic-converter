# Security and regulated-use position

## Status

This repository is **not certified compliant** with ISO/IEC 27001, FCA rules, UK GDPR, or any other standard. It is a technical component that has been designed to support a controlled deployment. Your firm’s Information Security, Data Protection, Compliance, and Change Management owners must approve its use before it processes live claim data.

## Secure-by-default behaviour

- The desktop app makes no network calls and sends no image data to a cloud service.
- It does not retain source images, thumbnails, telemetry, user identifiers, or image-content logs.
- JPEG output has camera metadata removed, including EXIF location data.
- The demonstration release writes output to the current Windows user's Desktop. A production deployment must configure `ManagedOutputDirectory` to an approved, access-controlled location.
- Outputs never overwrite existing files.

## Controls outside this codebase

The firm must provide these controls:

1. A managed, encrypted output location with least-privilege access and documented retention/deletion rules. Do not configure Downloads, a personal OneDrive, removable media, or an unapproved share.
2. Managed Windows devices, full-disk encryption, endpoint protection, patching, and restricted local administrator rights.
3. Code-signing of the release, approved software distribution, and application allow-listing where used.
4. A software bill of materials (SBOM), vulnerability monitoring, and a tested process for updating Magick.NET and its native dependencies.
5. A DPIA/privacy review to determine whether claim images include personal or special-category data, plus a record of processing activity and appropriate privacy notices.
6. A named service owner, change approval, incident procedure, user guidance, and a tested recovery/manual workaround.

## Security reporting

Do not report vulnerabilities in claim data or screenshots. Report them through the firm's approved security incident channel, including the version, reproduction steps, and impact only.
