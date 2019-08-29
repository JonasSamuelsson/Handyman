using Microsoft.AspNetCore.Http;
using System.Net;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    internal class ApiVersionValidator : IApiVersionValidator
    {
        private readonly IApiVersionReader _versionReader;
        private readonly IValidationStrategy _validationStrategy;

        public ApiVersionValidator(IApiVersionReader versionReader, IValidationStrategy validationStrategy)
        {
            _versionReader = versionReader;
            _validationStrategy = validationStrategy;
        }

        public bool Validate(HttpRequest request, out ValidationResult result)
        {
            var version = _versionReader.Read(request);

            var defaultVersion = request.Headers[HeaderNames.DefaultVersion].ToString();
            var optional = bool.Parse(request.Headers[HeaderNames.Optional]);
            var validVersions = request.Headers[HeaderNames.ValidVersions];

            var context = new ValidationContext
            {
                Optional = optional,
                ValidVersions = validVersions,
                Version = version
            };

            if (_validationStrategy.Validate(context))
            {
                result = new ValidationResult { MatchedVersion = context.MatchedVersion ?? defaultVersion };
                return true;
            }

            var title = context.ErrorMessage ?? $"Invalid api version, supported versions: {string.Join(", ", validVersions)}.";
            result = new ValidationResult { ProblemDetails = new ProblemDetails(HttpStatusCode.BadRequest, title) };
            return false;
        }
    }
}