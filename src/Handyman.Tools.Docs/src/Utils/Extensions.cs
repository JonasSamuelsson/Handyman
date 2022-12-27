using Markdig;
using Markdig.Extensions.AutoIdentifiers;
using Markdig.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Tools.Docs.Utils
{
    public static class Extensions
    {
        public static MarkdownDocument ToMarkdownDocument(this IEnumerable<string> lines)
        {
            var markdown = string.Join(Environment.NewLine, lines);
            var markdownPipelineBuilder = new MarkdownPipelineBuilder();
            markdownPipelineBuilder.Extensions.Add(new AutoIdentifierExtension(AutoIdentifierOptions.Default));
            var markdownPipeline = markdownPipelineBuilder.Build();
            return Markdown.Parse(markdown, markdownPipeline);
        }

        public static void UnIndentLines(this IList<string> lines)
        {
            var whitespaceCharacters = new[] { ' ', '\t' };

            while (true)
            {
                var chars = lines
                    .Where(x => x.Length != 0)
                    .Select(x => x[0])
                    .Distinct()
                    .Take(2)
                    .ToList();

                if (chars.Count != 1)
                    return;

                if (!whitespaceCharacters.Contains(chars[0]))
                    return;

                for (var i = 0; i < lines.Count; i++)
                {
                    var line = lines[i];

                    if (line.Length == 0)
                        continue;

                    line = line.Substring(1);

                    lines[i] = line;
                }
            }
        }
    }
}