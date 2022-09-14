using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.TypeInfoResolvers;
using System;

namespace Handyman.DataContractValidator
{
    public class DataContractGenerator
    {
        public static DataContractGeneratorOptions DefaultOptions { get; set; } = new DataContractGeneratorOptions();

        public DataContractGeneratorOptions Options { get; set; } = DefaultOptions.Clone();

        public string GenerateFor<T>()
        {
            return GenerateFor(typeof(T));
        }

        public string GenerateFor(Type type)
        {
            var builder = new CodeBuilder(Options);

            new TypeInfoResolverContext()
                .GetTypeInfo(type)
                .GetDataContractSyntaxNode()
                .GenerateCode(builder);

            return builder.Build();
        }
    }
}