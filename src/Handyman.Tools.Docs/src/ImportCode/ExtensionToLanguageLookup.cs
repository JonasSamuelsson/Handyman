using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.ImportCode
{
    public static class ExtensionToLanguageLookup
    {
        public static string GetLanguage(string extension)
        {
            extension = extension.ToLowerInvariant().TrimStart('.');
            return Dictionary.TryGetValue(extension, out var value) ? value : extension;
        }

        private static readonly Dictionary<string, string> Dictionary =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                {"cs", "csharp"},
                {"js", "javascript"},
                {"ps1", "powershell"},
                {"ts", "typescript"},
                {"yml", "yaml"}
            };
    }
}