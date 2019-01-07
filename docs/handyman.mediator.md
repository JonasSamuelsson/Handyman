# Handyman.Mediator

Handyman.Mediator is a library for loosely coupled in-process communication via messages.  
It's cross platform supporting `netstandard2.0`.

## Basics

Mediator has two kinds of messages

* Requests - dispatched to a single handler returning a response.
* Events - dispatched to multiple handlers.

The core interface & class used to dispatch messages is `IMediator` & `Mediator`.

``` csharp
public interface IMediator
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}
```

The `Mediator` class takes a service provider delegate and an optional configuration object as constructor dependencies.

There are extension methods available for `Publish` and `Send` that doesn't take a `CancellationToken`.

`Mediator` supports pipeline handlers that allows for code to be executed before the final request/event handler(s).

## Requests

Requests can be used both for commands and queries.  
First, create a message:

``` csharp
class Echo : IRequest<string>
{
    public string Message { get; set; }
}
```

Next, create a handler:

``` csharp
class EchoHandler : IRequestHandler<Echo, string>
{
    public Task<string> Handle(Echo request, CancellationToken cancellationToken)
    {
        return Task.FromResult(request.Message);
    }
}
```

Finally, send message:

``` csharp
var s = await mediator.Send(new Echo { Message = "Hello" });
Console.WriteLine(s);
```

### Request types

There are two types of request, those that return a value and those that don't.

* `IRequest<TResponse>` - returns a value.
* `IRequest` - does not return a value.

`IRequest` actually implements `IRequest<Void>` to simplify the execution pipeline.

All request handlers must implement `IRequestHandler<TRequest, TResponse>`.

``` csharp
public interface IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
```

In case the request doesn't have a response and/or if the handler isn't async there are some helper classes that can be used:

* `RequestHandler<TRequest>` - no response
* `SyncRequestHandler<TRequest>` - synchronous & no response
* `SyncRequestHandler<TRequest, TResponse>` - synchronous

## Events

First, create message:

``` csharp
public class Ping : IEvent {}
```

Next, create handlers:

``` csharp
public class PingHandler1 : IEventHandler<Ping>
{
    public Task Handle(Ping @event)
    {
        Console.WriteLine("Pong 1");
        return Task.CompletedTask;
    }
}

public class PingHandler2 : IEventHandler<Ping>
{
    public Task Handle(Ping @event)
    {
        Console.WriteLine("Pong 2");
        return Task.CompletedTask;
    }
}
```

Finally, publish message:

``` csharp
await mediator.Publish(new Ping());
```

## Pipelines

Pipelines enables the ability to execute code before the actual handler is executed.

Pipelines are supported by both requests and events, but needs to be enabled via the configuration object passed in to the mediator.

``` csharp
new Configuration
{
    EventPipelineEnabled = true,
    RequestPipelineEnabled = true
}
```

Pipeline handlers must implement one of the pipeline handler interfaces

``` csharp
public interface IRequestPipelineHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, Func<TRequest, CancellationToken, Task<TResponse>> next);
}

public interface IEventPipelineHandler<TEvent>
    where TEvent : IEvent
{
    Task Handle(TEvent @event, CancellationToken cancellationToken, Func<TEvent, CancellationToken, Task> next);
}
```