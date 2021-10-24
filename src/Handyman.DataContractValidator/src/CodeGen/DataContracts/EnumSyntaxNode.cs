using Handyman.DataContractValidator.Model;
using System.Linq;

namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class EnumSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => true;

        public EnumTypeInfo TypeInfo { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder
                .Add("new Enum")
                .AddLineBreak()
                .Add("{")
                .AddLineBreak()
                .IncreaseIndentation()
                .Add($"{nameof(Enum.Flags)} = {TypeInfo.IsFlags.ToString().ToLowerInvariant()},")
                .AddLineBreak()
                .Add($"{nameof(Enum.Nullable)} = {TypeInfo.IsNullable.ToString().ToLowerInvariant()},")
                .AddLineBreak()
                .Add($"{nameof(Enum.Values)} =")
                .AddLineBreak()
                .Add("{")
                .AddLineBreak()
                .IncreaseIndentation();

            var last = TypeInfo.Values.Last().Key;

            foreach (var kvp in TypeInfo.Values)
            {
                builder.Add($"{{ {kvp.Key}, \"{kvp.Value}\" }}");

                if (kvp.Key != last)
                {
                    builder.Add(",");
                }

                builder.AddLineBreak();
            }

            builder
                .DecreaseIndentation()
                .Add("}")
                .AddLineBreak()
                .DecreaseIndentation()
                .Add("}");
        }

        public static ISyntaxNode Create(EnumTypeInfo typeInfo)
        {
            return new EnumSyntaxNode
            {
                TypeInfo = typeInfo
            };
        }
    }
}