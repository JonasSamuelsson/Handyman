using Handyman.Tools.Outdated.Analyze;
using Handyman.Tools.Outdated.GenerateConfig;
using Handyman.Tools.Outdated.Report;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO.Abstractions;
using System.Threading.Tasks;

namespace Handyman.Tools.Outdated
{
    [Command(AppInfo.AppName)]
    [Subcommand(typeof(AnalyzeCommand))]
    [Subcommand(typeof(GenerateConfigCommand))]
    [Subcommand(typeof(ReportCommand))]
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IFileSystem, FileSystem>()
                .AddSingleton<IFileWriter, JsonFileWriter>()
                .AddSingleton<IFileWriter, MarkdownFileWriter>()
                .AddSingleton<ConfigReader>()
                .AddSingleton<ProjectAnalyzer>()
                .AddSingleton<ProjectLocator>()
                .AddSingleton<ProjectUtil>()
                .BuildServiceProvider(true);

            var app = new CommandLineApplication<Program>();

            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            try
            {
                return await app.ExecuteAsync(args);
            }
            catch (Exception exception)
            {
                var console = PhysicalConsole.Singleton;
                console.ForegroundColor = ConsoleColor.Red;
                console.WriteLine(exception is ApplicationException ? exception.Message : exception.ToString());
                console.ResetColor();

                return 1;
            }
        }

        public void OnExecute(CommandLineApplication app)
        {
            app.ShowHelp();
        }
    }
}