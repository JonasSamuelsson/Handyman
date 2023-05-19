using MediatorSamples.DynamicDispatch;
using MediatorSamples.EventFilter;
using MediatorSamples.EventFilterToggle;
using MediatorSamples.EventHandlerToggle;
using MediatorSamples.Events;
using MediatorSamples.GenericFilter;
using MediatorSamples.PipelineCustomization;
using MediatorSamples.Publisher;
using MediatorSamples.RequestFilter;
using MediatorSamples.RequestFilterToggle;
using MediatorSamples.RequestHandlerToggle;
using MediatorSamples.RequestResponse;
using MediatorSamples.RequestResponseExperiment;
using MediatorSamples.RequestResponseWhenAnyHandler;
using MediatorSamples.Sender;
using Spectre.Console;

namespace MediatorSamples;

public class Program
{
   public static async Task Main()
   {
      var samples = new Dictionary<string, Type?>
      {
         { "Basics - Request / response", typeof(RequestResponseSample) },
         { "Basics - Events", typeof(EventsSample) },
         { "Basics - Sender", typeof(SenderSample) },
         { "Basics - Publisher", typeof(PublisherSample) },
         { "Filters - Request filter", typeof(RequestFilterSample) },
         { "Filters - Event filter", typeof(EventFilterSample) },
         { "Filters - Generic filter", typeof(GenericFilterSample) },
         { "Dynamic dispatch", typeof(DynamicDispatchSample) },
         { "Feature toggling - Request handler", typeof(RequestHandlerToggleSample) },
         { "Feature toggling - Event handler", typeof(EventHandlerToggleSample) },
         { "Feature toggling - Request filter", typeof(RequestFilterToggleSample) },
         { "Feature toggling - Event filter", typeof(EventFilterToggleSample) },
         { "Request / response experiments", typeof(RequestResponseExperimentSample) },
         { "Request / response when any handler", typeof(RequestResponseWhenAnyHandlerSample) },
         { "Pipeline customization", typeof(PipelineCustomizationSample) },
         { "Exit", null }
      };

      var prompt = new SelectionPrompt<string> { PageSize = samples.Count }.AddChoices(samples.Keys);
      var selection = AnsiConsole.Prompt(prompt);
      var type = samples[selection];

      if (type is null)
      {
         return;
      }

      await ((Sample)Activator.CreateInstance(type)!).RunAsync(CancellationToken.None);
   }
}