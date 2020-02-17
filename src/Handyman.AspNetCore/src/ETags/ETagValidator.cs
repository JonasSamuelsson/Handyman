using System.Text.RegularExpressions;

namespace Handyman.AspNetCore.ETags
{
    internal class ETagValidator : IETagValidator
    {
        private static readonly Regex Regex = new Regex("^(W/)?\".+\"$", RegexOptions.Compiled);

        public bool IsValidETag(string candidate)
        {
            return Regex.IsMatch(candidate);
        }
    }
}