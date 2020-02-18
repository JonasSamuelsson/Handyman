using System.Diagnostics;

namespace Handyman.AspNetCore.ApiVersioning.Schemes.Literal
{
    [DebuggerDisplay("Literal: {Text}")]
    internal class LiteralApiVersionParser : IApiVersionParser
    {
        public bool TryParse(string candidate, out IApiVersion version)
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                version = null;
                return false;
            }

            version = new LiteralApiVersion(candidate);
            return true;
        }
    }
}