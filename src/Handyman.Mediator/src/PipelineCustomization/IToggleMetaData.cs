using System.Collections.Generic;

namespace Handyman.Mediator.PipelineCustomization
{
    public interface IToggleMetaData
    {
        string Name { get; }
        IEnumerable<string> Tags { get; }
    }
}