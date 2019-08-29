using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public class ValidationContext
    {
        public string ErrorMessage { get; set; }
        public bool Optional { get; internal set; }
        public string MatchedVersion { get; set; }
        public StringValues ValidVersions { get; internal set; }
        public string Version { get; internal set; }
    }
}