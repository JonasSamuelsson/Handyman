# Handyman.Mediator

Handyman.Mediator is a library for loosely coupled in-process communication via messages.  
It's cross platform targeting `netstandard2.0`.

## Basics

Mediator has two kinds of messages

* Requests - dispatched to a single handler returning a response.
* Events - dispatched to multiple handlers.

The core types used to dispatch messages are `IMediator` & `Mediator`.

``` csharp
public interface IMediator
{
    Task Publish<TEvent>(TEvent @event, CancellationToken cancellationToken) where TEvent : IEvent;
    Task<TResponse> Send<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken);
}
```

The `Mediator` class takes a service provider delegate and an optional configuration object as constructor dependencies.

There are extension methods available for `Publish` and `Send` that doesn't take a `CancellationToken`.

`Mediator` also has support for filters that allows for code to be executed before and/or after the final request/event handler(s).

## Configuration

There is a `Handyman.Mediator.DependencyInjection` package that simplifies configuring mediator and other types.

## Requests

Requests are used for request/response kind of work loads where the request is handled by exactly one handler that produces a response. Lets have a look with an example.  

First, we are defining a request message stating that the response should be of type `string`:

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

Finally, send the request message:

``` csharp
var s = await mediator.Send(new Echo { Message = "Hello" });
Console.WriteLine(s);
```

### Request types

There are two types of request, those that return a value and those that don't.

* `IRequest<TResponse>` - returns a value of type TResponse.
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

In case the request doesn't have a response and/or if the handler isn't async there are based classes that can be used:

* `RequestHandler<TRequest>` - no response, sync or async
* `RequestHandler<TRequest, TResponse>` - synch

## Events

Events can be used to notify other parts of the system that something has happened and are handled by zero to many handlers.

Lets defaine a event message:

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

Finally, publish the event message:

``` csharp
await mediator.Publish(new Ping());
```

## Filters

Filters enables the ability to execute code before and/or after the actual handler is executed.  
They can be used to inspect and/or modify the request/response and even terminate the execution pipeline.

Filters are supported by both requests and events and must implement one of the filter interfaces

``` csharp
public interface IRequestFilter<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    Task<TResponse> Execute(RequestFilterContext<TRequest> context, RequestFilterExecutionDelegate next);
}

public interface IEventFilter<TEvent>
    where TEvent : IEvent
{
    Task Execute(EventFilterContext<TEvent> context, EventFilterExecutionDelegate next);
}
```

Filter execution order can be controlled be implementing the `IOrderedFilter` interface.  
Filters are executed in ascending order and filters that doesn't provide an explicit order defaults to `zero`.

## Customization (advanced usage)

Request/event processing can be customized by providing custom request/event filter/handler providers.

### Fan out

Requests can be faned out by decorating the request type with `FanOutAttribute`. The response from the first handler that completes successfully will be used.

### Experiments

todo
