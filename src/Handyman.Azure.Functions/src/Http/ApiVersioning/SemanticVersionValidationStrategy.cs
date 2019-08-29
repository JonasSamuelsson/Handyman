namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    internal class SemanticVersionValidationStrategy : IValidationStrategy
    {
        public bool Validate(ValidationContext validationContext)
        {
            var parserResult = SemanticVersionParser.Parse(validationContext.ValidVersions);

            if (string.IsNullOrEmpty(validationContext.Version))
            {
                if (validationContext.Optional)
                    return true;

                validationContext.ErrorMessage = parserResult.ValidationErrorMessage;
                return false;
            }

            if (!SemanticVersionParser.TryParse(validationContext.Version, out var semanticVersion))
            {
                validationContext.ErrorMessage = parserResult.ValidationErrorMessage;
                return false;
            }

            foreach (var declaredVersion in parserResult.DeclaredVersions)
            {
                if (semanticVersion.Major != declaredVersion.SemanticVersion.Major)
                    continue;

                var comparison =
                    SemanticVersionComparer.Default.Compare(semanticVersion, declaredVersion.SemanticVersion);

                if (comparison > 0)
                    continue;

                validationContext.MatchedVersion = declaredVersion.String;
                return true;
            }

            validationContext.ErrorMessage = parserResult.ValidationErrorMessage;
            return false;
        }
    }
}