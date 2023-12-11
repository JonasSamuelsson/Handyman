using Handyman.Mediator;
using Handyman.Mediator.Pipeline.RequestFilterToggle;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.RequestFilterToggle;

public class RequestFilterToggleSample : Sample
{
    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        await mediator.Send(new Request(), cancellationToken);
        await mediator.Send(new Request(), cancellationToken);
    }
}

[RequestFilterToggle<Filter>]
public class Request : IRequest<string>
{
}

public class Filter : IRequestFilter<Request, string>
{
    public async Task<string> Execute(RequestContext<Request> requestContext, RequestFilterExecutionDelegate<string> next)
    {
        Console.WriteLine("Filter");
        return await next();
    }
}

public class Handler : IRequestHandler<Request, string>
{
    public Task<string> Handle(Request request, CancellationToken cancellationToken)
    {
        Console.WriteLine("Handler");
        return Task.FromResult("");
    }
}

public class Toggle : IRequestFilterToggle
{
    private static bool _enabled;

    public Task<bool> IsEnabled<TRequest, TResponse>(RequestFilterToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>
    {
        _enabled = !_enabled;
        return Task.FromResult(_enabled);
    }
}