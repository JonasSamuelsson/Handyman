using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.TableOfContent;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

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
        }

        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}
