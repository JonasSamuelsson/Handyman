# Handyman.AspNetCore changelog

## 2.0.0

### Added

* Added integration with asp.net core routing system.
* ETag convertion/comparison/validation can be replaced with custom implementations using dependency injection.

### Changed

* Changed how the default versioning scheme works.
  * Still uses the same format but requires exact major/minor match, ie `1` does not match `1.0`.

### Removed

* Removed asp.net core 2.x support.