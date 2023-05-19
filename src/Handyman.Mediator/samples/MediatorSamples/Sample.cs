using Handyman.Mediator.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;

namespace MediatorSamples;

public abstract class Sample
{
   public abstract Task RunAsync(CancellationToken cancellationToken);

   private readonly Lazy<IServiceProvider> _serviceProvider;

   protected Sample()
   {
      _serviceProvider = new(ConfigureServices);
   }

   public IServiceProvider ServiceProvider => _serviceProvider.Value;

   private IServiceProvider ConfigureServices()
   {
      var sampleType = GetType();

      var types = sampleType.Assembly.GetTypes()
         .Where(x => x.Namespace == sampleType.Namespace)
         .ToList();

      var serviceCollection = new ServiceCollection().AddMediator(x => x.ScanTypes(types));

      return serviceCollection.BuildServiceProvider();
   }
}