﻿using Handyman.DataContractValidator.CodeGen.DataContracts;
using Shouldly;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen.DataContracts
{
    public class ValueSyntaxNodeTests
    {
        [Fact]
        public void ShouldBeSingleLine()
        {
            new ValueSyntaxNode().IsMultiLine.ShouldBeFalse();
        }

        [Fact]
        public void ShouldGenerateCode()
        {
            new ValueSyntaxNode { TypeName = "bool?" }
                .GenerateCode()
                .ShouldBe("bool?");
        }
    }
}