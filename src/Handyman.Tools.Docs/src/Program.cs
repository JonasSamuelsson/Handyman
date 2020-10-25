using Handyman.DependencyInjection;
using Handyman.Tools.Docs.ImportCode;
using Handyman.Tools.Docs.TableOfContent;
using Handyman.Tools.Docs.Utils;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Handyman.Tools.Docs
{
    [Subcommand(typeof(TableOfContentCommand))]
    [Subcommand(typeof(CodeBlocksCommand))]
    public class Program
    {
        public static Task<int> Main(string[] args)
            => CreateHostBuilder()
                .RunCommandLineApplicationAsync<Program>(args);

        public static Task<int> Run(Action<IServiceCollection> configureServices, string[] args)
            => CreateHostBuilder()
                .ConfigureServices(configureServices)
                .RunCommandLineApplicationAsync<Program>(args);

        private static IHostBuilder CreateHostBuilder()
            => new HostBuilder()
                .ConfigureServices(ConfigureServices);

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IFileSystem, FileSystem>();
            services.AddSingleton<ILogger, Logger>();

            services.Scan(x =>
            {
                x.AssemblyContaining<Program>();
                x.ConfigureConcreteClassesOf(typeof(IElementSerializer<>));
                x.ConfigureConcreteClassesOf(typeof(IDataSerializer<>));
            });
        }

        public void OnExecute(CommandLineApplication app) => app.ShowHelp();
    }
}
