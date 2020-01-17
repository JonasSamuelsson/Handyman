using Handyman.Tools.Outdated.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;
using Handyman.Tools.Outdated.Analyze;

namespace Handyman.Tools.Outdated
{
    [Command("handyman-outdated")]
    [Subcommand(typeof(AnalyzeCommand))]
    public class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
                .AddSingleton<IProcessRunner, ProcessRunner>()
                .AddSingleton<ProjectAnalyzer>()
                .AddSingleton<ProjectResolver>()
                .BuildServiceProvider(true);

            var app = new CommandLineApplication<Program>();

            app.Conventions
                .UseDefaultConventions()
                .UseConstructorInjection(services);

            try
            {
                return app.Execute(args);
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
