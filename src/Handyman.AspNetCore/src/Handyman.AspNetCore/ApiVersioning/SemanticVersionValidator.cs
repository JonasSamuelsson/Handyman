using Microsoft.Extensions.Primitives;

namespace Handyman.AspNetCore.ApiVersioning
{
    internal class SemanticVersionValidator : IApiVersionValidator
    {
        public bool Validate(string version, bool optional, StringValues validVersions, out string matchedVersion, out string error)
        {
            matchedVersion = null;
            error = null;

            var parserResult = SemanticVersionParser.Parse(validVersions);

            if (string.IsNullOrEmpty(version))
            {
                if (optional)
                    return true;

                error = parserResult.ValidationErrorMessage;
                return false;
            }

            if (!SemanticVersionParser.TryParse(version, out var semanticVersion))
            {
                error = parserResult.ValidationErrorMessage;
                return false;
            }

            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < parserResult.DeclaredVersions.Length; i++)
            {
                var declaredVersion = parserResult.DeclaredVersions[i];

                if (semanticVersion.Major != declaredVersion.SemanticVersion.Major)
                    continue;

                var comparison = SemanticVersionComparer.Default.Compare(semanticVersion, declaredVersion.SemanticVersion);

                if (comparison > 0)
                    continue;

                matchedVersion = declaredVersion.String;
                return true;
            }

            error = parserResult.ValidationErrorMessage;
            return false;
        }
    }
}