using Handyman.DependencyInjection;
using Handyman.Tools.Docs.BuildTablesOfContents;
using Handyman.Tools.Docs.ImportCodeBlocks;
using Handyman.Tools.Docs.ImportContent;
using Handyman.Tools.Docs.Shared;
using Handyman.Tools.Docs.ValidateLinks;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.IO.Abstractions;

namespace Handyman.Tools.Docs;

public class Program
{
    public static int Main(string[] args)
    {
        return Run(args, delegate { });
    }

    public static int Run(string[] args, Action<IServiceCollection> action)
    {
        var services = new ServiceCollection();

        services.AddSingleton(AnsiConsole.Console);
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddSingleton<ILogger, ConsoleLogger>();

        services.Scan(s =>
        {
            s.AssemblyContaining<Program>();
            s.ConfigureConcreteClassesOf<IValueParser>();
            s.ConfigureDefaultImplementations();
        });

        action.Invoke(services);

        var app = new CommandApp(new TypeRegistrar(services));

        app.Configure(root =>
        {
            root.AddCommand<BuildTablesOfContentsCommand>("build-tables-of-contents");
            root.AddCommand<ImportCodeBlocksCommand>("import-code-blocks");
            root.AddCommand<ImportContentCommand>("import-content");
            root.AddCommand<ValidateLinksCommand>("validate-links");
        });

        try
        {
            return app.Run(args);
        }
        catch (Exception exception)
        {
            AnsiConsole.WriteException(exception);
            return -1;
        }
    }
}