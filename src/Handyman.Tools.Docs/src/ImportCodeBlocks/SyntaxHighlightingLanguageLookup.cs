using System;
using System.Collections.Generic;

namespace Handyman.Tools.Docs.ImportCodeBlocks
{
    public static class SyntaxHighlightingLanguageLookup
    {
        public static string GetSyntaxHighlightingLanguage(string extension)
        {
            extension = extension.ToLowerInvariant().TrimStart('.');
            return Dictionary.TryGetValue(extension, out var value) ? value : extension;
        }

        private static readonly Dictionary<string, string> Dictionary = new(StringComparer.OrdinalIgnoreCase)
        {
            { "cs", "csharp" },
            { "js", "javascript" },
            { "ps1", "powershell" },
            { "ts", "typescript" },
            { "yml", "yaml" }
        };
    }
}