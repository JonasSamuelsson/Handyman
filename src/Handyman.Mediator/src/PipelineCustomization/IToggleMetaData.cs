using System.Collections.Generic;

namespace Handyman.Mediator.PipelineCustomization
{
    public interface IToggleMetadata
    {
        string Name { get; }
        IEnumerable<string> Tags { get; }
    }
}