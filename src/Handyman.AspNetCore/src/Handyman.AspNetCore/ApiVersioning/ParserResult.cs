namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ParserResult
    {
        public Version[] DeclaredVersions { get; set; }
        public string ValidationError { get; set; }
        internal class Version
        {
            public SemanticVersion SemanticVersion { get; set; }
            public string String { get; set; }
        }
    }
}