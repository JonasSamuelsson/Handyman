using System.Threading.Tasks;

namespace Handyman.Mediator.Pipeline
{
    public interface IToggle
    {
        Task<bool> IsEnabled(IToggleMetadata toggleMetadata, MessageContext pipelineContext);
    }
}