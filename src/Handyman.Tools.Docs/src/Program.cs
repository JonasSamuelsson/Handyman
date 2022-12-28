using Handyman.DependencyInjection;
using Handyman.Tools.Docs.BuildTablesOfContents;
using Handyman.Tools.Docs.ImportCodeBlocks;
using Handyman.Tools.Docs.Shared;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO.Abstractions;

namespace Handyman.Tools.Docs
{
    public class Program
    {
        public static int Main(string[] args)
        {
            return Run(args, delegate { });
        }

        public static int Run(string[] args, Action<IServiceCollection> action)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IFileSystem, FileSystem>();

            services.Scan(s =>
            {
                s.AssemblyContaining<Program>();
                s.ConfigureConcreteClassesOf<IValueConverter>();
                s.ConfigureDefaultImplementations();
            });

            action.Invoke(services);

            var app = new CommandApp(new TypeRegistrar(services));

            app.Configure(root =>
            {
                root.AddCommand<BuildTablesOfContentsCommand>("build-tables-of-contents");
                root.AddCommand<ImportCodeBlocksCommand>("import-code-blocks");
            });

            try
            {
                return app.Run(args);
            }
            catch (Exception exception)
            {
                AnsiConsole.WriteException(exception);
                throw;
            }
        }
    }
}