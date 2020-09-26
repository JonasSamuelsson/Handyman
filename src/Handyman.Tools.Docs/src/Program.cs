using Handyman.DependencyInjection;
using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.TableOfContent;
using Handyman.Tools.Docs.Utils;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.IO.Abstractions;

namespace Handyman.Tools.Docs
{
    [Subcommand(typeof(TableOfContentCommand))]
    [Subcommand(typeof(ImportCodeCommand))]
    public class Program
    {
        public static void Main(string[] args)
            => new HostBuilder()
                .ConfigureServices(ConfigureServices)
                .RunCommandLineApplicationAsync<Program>(args);

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFileSystem, FileSystem>();
            services.AddSingleton<ElementsParser>();

            services.Scan(x =>
            {
                x.AssemblyContaining<Program>();
                x.ConfigureConcreteClassesOf<IAttributesParser>();
            });
        }

        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}
