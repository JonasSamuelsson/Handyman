using System.Collections.Generic;

namespace Handyman.Mediator.Pipelines
{
    public interface IToggleMetadata
    {
        string Name { get; }
        IEnumerable<string> Tags { get; }
    }
}