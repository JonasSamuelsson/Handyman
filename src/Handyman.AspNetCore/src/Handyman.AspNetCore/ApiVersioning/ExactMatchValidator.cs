using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    public class ExactMatchValidator : IApiVersionValidator
    {
        public bool Validate(string version, bool optional, StringValues validVersions, out string matchedVersion,
            out string customProblemDetail)
        {
            matchedVersion = null;
            customProblemDetail = null;

            if (optional && version == string.Empty)
                return true;

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < validVersions.Count; i++)
            {
                var validVersion = validVersions[i];
                if (version != validVersion) continue;
                matchedVersion = validVersion;
                return true;
            }

            customProblemDetail = $"Invalid api version, supported versions: {validVersions}.";
            return false;
        }
    }
}