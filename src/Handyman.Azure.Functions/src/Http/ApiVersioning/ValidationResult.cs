namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public class ValidationResult
    {
        public string MatchedVersion { get; set; }
        public ProblemDetails ProblemDetails { get; set; }
    }
}