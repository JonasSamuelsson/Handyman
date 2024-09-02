using Microsoft.AspNetCore.Mvc.Filters;

namespace Handyman.AspNetCore.ETags.Internals;

internal class ETagMetadata : IFilterMetadata
{
    public string HeaderName { get; set; }
    public bool IsRequired { get; set; }
}