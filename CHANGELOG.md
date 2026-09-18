# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-09-18

### 🚀 Added
- **Multi-Monitor DPI Awareness**: Real-time cursor screen detection with `SHCore.dll` / `MonitorFromPoint` and Per-Monitor V2 DPI scaling.
- **Multi-Group Item Assignment**: Shortcuts can belong to multiple categories simultaneously without duplication.
- **Context-Aware Group Management**: Directly manage and detach group memberships from the contextual menu.
- **Automated CI/CD Pipeline**: GitHub Actions matrix workflow testing Debug and Release across multiple Windows runner environments.
- **Community Governance & Templates**: Issue forms for structured Bug Reports and Feature Requests, plus release note categorization.
- **Project Metadata & Packaging**: Native `.nuspec`, `Directory.Build.props`, `AssemblyInfo.cs` and `package.json` configurations.

### 🐛 Fixed
- Fixed bug in group lookup causing items not to render in assigned groups.
- Resolved DPI scaling offsets and window bounds issues across mixed-DPI displays.
- Fixed tray icon and window state handling during silent startup (`--minimized`).

### ⚡ Performance & Polish
- Process priority elevated for instantaneous hotkey overlay invocation.
- Refactored overlay show/hide lifecycle with `ShowOverlay()` and `HideOverlay()`.

---

## [1.0.0] - 2023-10-15
- Initial public release with customizable hotkey, video thumbnailing via FFmpeg, and System Tray integration.
