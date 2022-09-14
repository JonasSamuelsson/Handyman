# Handyman.DataContractValidator changelog

## 4.0.0 - 2022-09-14

* Added ability to control property name comparison during validation.
* Added ability to control enum value name comparison during validation.
* Added ability to generate data contracts with properties sorted in alphabetical order.
* Added static `DefaultOptions`property to both `DataContractGenerator` and `DataContractValidator` (configure all instances).
* Added `Options`property to both `DataContractGenerator` and `DataContractValidator` (configure single instance).
* Removed `DataContractStore` property from `DataContractValidator`.
* Renamed `ValidationOptions` to `DataContractValidatorOptions`.

## 3.2.0 - 2022-06-28

* Validate recursive types.
* Expose `DataContractStore` directly on `DataContractValidator` for better discoverability.

## 3.1.1 - 2022-06-17

* Handle enums where same underlying value is used multiple times.

## 3.1.0 - 2022-04-06

* Added ability to allow properties not found in data contract during validation.

## 3.0.0 - 2022-03-05

* Added nullable reference type awareness.
* Changed how enums are declared.

## 2.2.0 - 2020-09-16

### Added

* Added support for properties of any type.
