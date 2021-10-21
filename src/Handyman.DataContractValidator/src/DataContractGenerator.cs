using Handyman.DataContractValidator.CodeGen;
using Handyman.DataContractValidator.TypeInfoResolvers;
using System;

namespace Handyman.DataContractValidator
{
    public class DataContractGenerator
    {
        public string GenerateFor<T>()
        {
            return GenerateFor(typeof(T));
        }

        public string GenerateFor(Type type)
        {
            var builder = new CodeBuilder();

            new TypeInfoResolverContext()
                .GetTypeInfo(type)
                .GetDataContractSyntaxNode()
                .GenerateCode(builder);

            return builder.Build();
        }
    }
}