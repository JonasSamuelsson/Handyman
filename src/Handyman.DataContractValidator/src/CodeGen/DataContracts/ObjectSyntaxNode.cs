using Handyman.DataContractValidator.Model;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class ObjectSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => Properties.Any();

        public IEnumerable<PropertyInitializerSyntaxNode> Properties { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            if (!Properties.Any())
            {
                GenerateObjectWithoutProperties(builder);
            }
            else
            {
                GenerateObjectWithProperties(builder);
            }
        }

        private void GenerateObjectWithoutProperties(CodeBuilder builder)
        {
            builder.Add("new { }");
        }

        private void GenerateObjectWithProperties(CodeBuilder builder)
        {
            builder
                .Add("new")
                .AddLineBreak()
                .Add("{")
                .AddLineBreak()
                .IncreaseIndentation();

            var last = Properties.Last();

            foreach (var property in Properties)
            {
                builder.Add(property);

                if (property != last)
                {
                    builder.Add(",");
                }

                builder.AddLineBreak();
            }

            builder
                .DecreaseIndentation()
                .Add("}");
        }

        public static ISyntaxNode Create(ObjectTypeInfo typeInfo)
        {
            return new ObjectSyntaxNode
            {
                Properties = typeInfo.Properties
                    .Where(x => !x.IsIgnored)
                    .Select(x => new PropertyInitializerSyntaxNode
                    {
                        PropertyName = x.Name,
                        PropertyValue = x.Type.GetDataContractSyntaxNode().WrapWithTypeOfIfValueSyntaxNode()
                    })
                    .ToList()
            };
        }
    }
}