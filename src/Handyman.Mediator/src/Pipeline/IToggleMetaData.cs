using System.Collections.Generic;

namespace Handyman.Mediator.Pipeline
{
    public interface IToggleMetadata
    {
        string? Name { get; }
        IEnumerable<string>? Tags { get; }
    }
}