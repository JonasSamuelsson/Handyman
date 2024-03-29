﻿namespace Handyman.DataContractValidator.CodeGen.DataContracts
{
    internal class PropertyInitializerSyntaxNode : ISyntaxNode
    {
        public bool IsMultiLine => Value.IsMultiLine;

        public string Name { get; set; }
        public ISyntaxNode Value { get; set; }

        public void GenerateCode(CodeBuilder builder)
        {
            builder
                .Add($"{Name} = ")
                .Add(Value);
        }
    }
}