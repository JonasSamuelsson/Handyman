using ConsoleTables;
using McMaster.Extensions.CommandLineUtils;
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Handyman.Tools.Ado
{
    [Command("list-repos")]
    public class ListRepositoriesCommand
    {
        [Option(Description = "Host (defaults to dev.azure.com)", ShortName = "")]
        public string Host { get; set; }

        [Option(ShortName = ""), Required]
        public string Organization { get; set; }

        [Option(ShortName = ""), Required]
        public string Project { get; set; }

        [Option(ShortName = "pat"), Required]
        public string PersonalAccessToken { get; set; }

        [Option(ShortName = "of")]
        public OutputFormat OutputFormat { get; set; } = OutputFormat.List;

        public async Task OnExecute()
        {
            var host = Host ?? "dev.azure.com";
            var uri = new Uri($"https://{host}/{Organization}/{Project}/_apis/git/repositories?api-version=5.1");
            var request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($":{PersonalAccessToken}")));
            var response = await new HttpClient().SendAsync(request);
            var content = await response.Content.ReadAsAsync<ResponseContent<Repository>>();

            var repos = content.Value.Select(x => new { x.Name, Url = x.WebUrl }).ToList();

            switch (OutputFormat)
            {
                case OutputFormat.List:
                    repos.ForEach(x => Console.WriteLine(x.Url));
                    break;
                case OutputFormat.Table:
                    Console.WriteLine(ConsoleTable.From(repos).ToMinimalString());
                    break;
                case OutputFormat.Json:
                    Console.WriteLine(JsonConvert.SerializeObject(repos, Formatting.Indented));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}