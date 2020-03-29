# Handyman.AspNetCore changelog

## 2.0.0

### Added

* Added integration with asp.net core routing system.
* Added `major-minor-prerelease` versioning scheme.
* ETag convertion/comparison/validation can be replaced with custom implementations.

### Changed

* ETag implementation is now designed to work with dependecy injection.

### Removed

* Removed `semver` versioning scheme.
