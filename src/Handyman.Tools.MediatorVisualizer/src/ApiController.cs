using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Handyman.Tools.MediatorVisualizer
{
    [ApiController, Route("api")]
    public class ApiController : ControllerBase
    {
        private static AnalyzerResult AnalyzerResult;

        [HttpGet("test")]
        public object Test() => new { Message = "Wohoo =)" };

        [HttpGet("analyze/{project}")]
        public async Task<object> Analyze(string project)
        {
            AnalyzerResult = await Analyzer.Analyze(project);

            return new
            {
                AnalyzerResult.EntryPoints
            };
        }

        [HttpGet("graph/{item}")]
        public string Graph(string item, string layout = "TB")
        {
            return GraphGenerator.GenerateGraph(item, layout, AnalyzerResult);
        }
    }
}
