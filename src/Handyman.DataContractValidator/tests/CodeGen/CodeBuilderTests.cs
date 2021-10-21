using Handyman.DataContractValidator.CodeGen;
using Shouldly;
using System;
using Xunit;

namespace Handyman.DataContractValidator.Tests.CodeGen
{
    public class CodeBuilderTests
    {
        private static readonly string NewLine = Environment.NewLine;

        [Fact]
        public void ShouldAddToSameLine()
        {
            new CodeBuilder()
                .Add("one")
                .Add(" ")
                .Add("two")
                .Build()
                .ShouldBe("one two");
        }

        [Fact]
        public void ShouldAddLineBreaks()
        {
            new CodeBuilder()
                .Add("one")
                .AddLineBreak()
                .Add("two")
                .Build()
                .ShouldBe($"one{NewLine}two");
        }

        [Fact]
        public void ShouldUseIndentationFromOptions()
        {
            new CodeBuilder(new CodeBuilderOptions { Indentation = " " })
                .IncreaseIndentation()
                .Add("x")
                .Build()
                .ShouldBe(" x");

            new CodeBuilder(new CodeBuilderOptions { Indentation = "  " })
                .IncreaseIndentation()
                .Add("x")
                .Build()
                .ShouldBe("  x");
        }

        [Fact]
        public void LinesShouldBePrefixedWithCurrentIndentationLevel()
        {
            new CodeBuilder(new CodeBuilderOptions { Indentation = " " })
                .Add("one")
                .AddLineBreak()
                .IncreaseIndentation()
                .Add("two")
                .AddLineBreak()
                .IncreaseIndentation()
                .Add("three")
                .AddLineBreak()
                .DecreaseIndentation()
                .Add("four")
                .Build()
                .ShouldBe($"one{NewLine} two{NewLine}  three{NewLine} four");
        }

        [Fact]
        public void EmptyLinesShouldNotBeIndented()
        {
            new CodeBuilder(new CodeBuilderOptions { Indentation = " " })
                .IncreaseIndentation()
                .Add("one")
                .AddLineBreak()
                .AddLineBreak()
                .Add("two")
                .Build()
                .ShouldBe($" one{NewLine}{NewLine} two");
        }

        [Fact]
        public void TheOrderOfChangingIndentationAndAddingLineBreaksShouldNotMatter()
        {
            new CodeBuilder(new CodeBuilderOptions { Indentation = " " })
                .IncreaseIndentation()
                .AddLineBreak()
                .Add("one")
                .Build()
                .ShouldBe($"{NewLine} one");

            new CodeBuilder(new CodeBuilderOptions { Indentation = " " })
                .AddLineBreak()
                .IncreaseIndentation()
                .Add("one")
                .Build()
                .ShouldBe($"{NewLine} one");
        }
    }
}