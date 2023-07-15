using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;

namespace Handyman.AspNetCore.ApiVersioning;

// ReSharper disable once RedundantAttributeUsageProperty
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public sealed class ApiVersionAttribute : Attribute, IFilterMetadata
{
    private readonly StringValues _versions;

    public ApiVersionAttribute(string version)
        : this(new[] { version })
    {
    }

    public ApiVersionAttribute(string[] versions)
    {
        _versions = versions;
    }

    /// <summary>
    /// Specify the default version to use if api version is optional and not provided in the request.
    /// </summary>
    public string DefaultVersion { get; set; }

    /// <summary>
    /// Specify if an api version is optional or not, default is false.
    /// </summary>
    public bool Optional
    {
        get => NullableOptional ?? false;
        set => NullableOptional = value;
    }

    internal bool? NullableOptional { get; set; }
    internal StringValues Versions => _versions;
}