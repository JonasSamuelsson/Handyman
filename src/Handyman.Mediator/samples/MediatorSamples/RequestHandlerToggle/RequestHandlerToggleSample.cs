using Handyman.Mediator;
using Handyman.Mediator.Pipeline.RequestHandlerToggle;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples.RequestHandlerToggle;

public class RequestHandlerToggleSample : Sample
{
    public override async Task RunAsync(CancellationToken cancellationToken)
    {
        var mediator = ServiceProvider.GetRequiredService<IMediator>();

        var request1 = new GetString();

        var response1 = await mediator.Send(request1, cancellationToken);

        Console.WriteLine(response1);

        var request2 = new GetString();

        var response2 = await mediator.Send(request2, cancellationToken);

        Console.WriteLine(response2);
    }
}

[RequestHandlerToggle<GetDateTimeString, GetDateString>]
public class GetString : IRequest<string>
{
}

public class GetDateString : IRequestHandler<GetString, string>
{
    public Task<string> Handle(GetString request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd"));
    }
}

public class GetDateTimeString : IRequestHandler<GetString, string>
{
    public Task<string> Handle(GetString request, CancellationToken cancellationToken)
    {
        return Task.FromResult(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}

public class Toggle : IRequestHandlerToggle
{
    private static bool _enabled;

    public Task<bool> IsEnabled<TRequest, TResponse>(RequestHandlerToggleMetadata toggleMetadata, RequestContext<TRequest> requestContext) where TRequest : IRequest<TResponse>
    {
        _enabled = !_enabled;
        return Task.FromResult(_enabled);
    }
}