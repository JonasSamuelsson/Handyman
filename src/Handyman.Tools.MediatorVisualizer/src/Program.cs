using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Build.Locator;
using Microsoft.Extensions.Hosting;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Handyman.Tools.MediatorVisualizer
{
    public class Program
    {
        public static Task<int> Main(string[] args) => CommandLineApplication.ExecuteAsync<Program>(args);

        [Option(ShortName = "")]
        public int? Port { get; set; }

        [Option(CommandOptionType.NoValue, ShortName = "")]
        public bool NoLaunchBrowser { get; set; }

        public async Task OnExecute()
        {
            MSBuildLocator.RegisterDefaults();

            Port ??= 63636;

            var task = CreateHostBuilder().Build().RunAsync();

            if (NoLaunchBrowser == false)
            {
                Process.Start(new ProcessStartInfo(Url)
                {
                    UseShellExecute = true
                });
            }

            await task;
        }

        public IHostBuilder CreateHostBuilder() =>
            Host.CreateDefaultBuilder()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(Url);
                });

        private string Url => $"http://localhost:{Port}/";
    }
}
