# Handyman.AspNetCore changelog

## 3.3.0 - 2021-05-19

### Added

* Added `IETagUtilities` so only a single dependency is required to use both `IETagComparer` & `IETagConverter`.

## 3.2.0

### Added

* Added .net5.0 TFM.

## 3.1.2

### Fixed

* Fixed null reference exception in etag validator middleware.

## 3.1.1

### Fixed

* Fixed api version routing issue.

## 3.1.0

### Added

* Added `IETagConverter`.

## 3.0.0

### Added

* Added `EnsureEquals(...)` methods to `IETagComparer`.
* Started using middleware for e-tag request validation and error handling.

### Removed

* Removed `IETagConverter` & `IETagValidator`, these are now implementation details.

### Fixed

* Returning the correct http status code if the `If-Match` header isn't provided.

## 2.0.0

### Added

* Added integration with asp.net core routing system.
* ETag convertion/comparison/validation can be replaced with custom implementations using dependency injection.

### Changed

* Changed how the default versioning scheme works.
  * Still uses the same format but requires exact major/minor match, ie `1` does not match `1.0`.

### Removed

* Removed asp.net core 2.x support.
