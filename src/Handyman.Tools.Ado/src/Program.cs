using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Handyman.Tools.Ado
{
    [Command("handyman-ado")]
    [HelpOption]
    [Subcommand(typeof(ListRepositoriesCommand))]
    public class Program
    {
        public static int Main(string[] args)
        {
            var services = new ServiceCollection()
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
