# Handyman.Mediator changelog

## 11.3.0 - 2021-12-23

* Added support for specifying toggle failure mode (`disabled`/`enabled`/`throw`).

## 11.2.1 - 2021-08-23

* Fixed `request`/`event` pipeline builder concurrency issue.

## 10.0.0 - 2020-09-20

### Added

* Added event and request dispatcher interfaces to mitigate the service locator pattern.
* Added event and request pipeline builder execution ordering.
* Added `ToggleBase`, which can handle all toggle state resolution.

### Changed

* `IRequestHandlerExecutionStrategy` takes a single `IRequestHandler<,>`.
* Removed generic type constraints from event and request filters, making them more powerfull in combination with a container supporting generic type constraints.
* Redesigned the event and pipeline customization api to make it easier to combine multiple customizations.
* Redesigned current pipeline customizations to so they can be combined with other customizations.
* Renamed all `*TobbleInfo` classes to `*ToggleMetadata`.
* All toggles can enable/disable multiple filters/handlers.
* Merged `EventPipelineCustomization` and `RequestPipelineCustomization` namespaces to `PipelineCustomization`.

### Removed

* `IMediator` `Publish` and `Send` extension methods without cancellation token.

## 9.0.0 - 2020-06-22

### Changed

* All toggle attributes uses `null` as the default value for the `Tags` property.

## 8.1.3

### Fixed

* Request handler experiment execution uses global continue on captured context configuration.

## 8.1.2

### Fixed

* Request handler experiment exception handling.
