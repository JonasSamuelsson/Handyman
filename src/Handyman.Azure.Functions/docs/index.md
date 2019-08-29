# Handyman.Azure.Functions

This package provides

* Http endpoint api versioning
* Function result
* Problem details

## Http endpoint api versioning

Register required services to enable api versioning.

``` csharp
builder.Services.AddApiVersioning();
```

Decorate your http triggered functions with `ApiVersionAttribute`.

``` csharp
[ApiVersion("1.2")]
[ApiVersion(new [] { "1.0", "2.0" }, Optional = true, DefaultVersion = "1.0")]
```

Finally take a dependency on `IApiVersionValidator` and validate the request.

``` csharp
public class MyFunctionClass
{
    private readonly IApiVersionValidator _apiVersionValidator;

    public MyFunctionClass(IApiVersionValidator apiVersionValidator)
    {
        _apiVersionValidator = apiVersionValidator;
    }

    [ApiVersion("1.0")]
    public FunctionResult Ping(HttpRequest request)
    {
        if (!_apiVersionValidator.Validate(request, out var result))
            return result.ProblemDetails;
    }
}
```

## Function result

todo

## Problem details