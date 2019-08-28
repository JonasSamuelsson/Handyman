using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    public class ApiVersionValidator : IApiVersionValidator
    {
        private readonly IApiVersionReader _versionReader;
        private readonly IApiVersionValidationStrategy _validationStrategy;

        public ApiVersionValidator(IApiVersionReader versionReader, IApiVersionValidationStrategy validationStrategy)
        {
            _versionReader = versionReader;
            _validationStrategy = validationStrategy;
        }

        public bool Validate(HttpRequest request, bool optional, StringValues validVersions, out string matchedVersion, out string error)
        {
            var version = _versionReader.Read(request);
            return _validationStrategy.Validate(version, optional, validVersions, out matchedVersion, out error);
        }
    }
}