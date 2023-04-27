# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## Unreleased

### Added

- Error handling for unexpected errors during sort (e.g., file in use by another process).

## [2.1.0] - 2023-04-15

### Added

- Skip directories feature.
- New alerts when: no sources set, no output directory set, skipped episode/series/directory.
- Github link to Help Menu.

### Changed

- Removed usage guide from README. Will get video tutorials soon.
- Removed alert when no new video files found.

## [2.0.0] - 2023-04-02

**The Great SQLite Migration**

### Added

- Software now uses SQLite db instead of text files for settings.
- Automatic import of old settings.

### Fixed

- Sanitization of series titles where special characters were crashing the software.

## [1.1.0] - 2022-12-06

**Happy GUI Edition**

- Initial Release. No changelog to be found here, sorry.