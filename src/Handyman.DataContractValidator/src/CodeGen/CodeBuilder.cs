using System.Linq;
using System.Text;

namespace Handyman.DataContractValidator.CodeGen
{
    internal class CodeBuilder
    {
        private readonly StringBuilder _builder = new StringBuilder();
        private int _indentationLevel;
        private bool _isNewLine = true;

        public CodeBuilder() : this(new DataContractGeneratorOptions())
        {
        }

        public CodeBuilder(DataContractGeneratorOptions options)
        {
            Options = options;
        }

        public DataContractGeneratorOptions Options { get; }

        public CodeBuilder Add(ISyntaxNode node)
        {
            node.GenerateCode(this);
            return this;
        }

        public CodeBuilder Add(string code)
        {
            if (_isNewLine)
            {
                _builder.Append(string.Join("", Enumerable.Repeat(Options.Indentation, _indentationLevel)));
                _isNewLine = false;
            }

            _builder.Append(code);

            return this;
        }

        public CodeBuilder AddLineBreak()
        {
            _isNewLine = true;
            _builder.AppendLine();
            return this;
        }

        public CodeBuilder IncreaseIndentation()
        {
            return ChangeIndentation(1);
        }

        public CodeBuilder DecreaseIndentation()
        {
            return ChangeIndentation(-1);
        }

        private CodeBuilder ChangeIndentation(int delta)
        {
            _indentationLevel += delta;
            return this;
        }

        public string Build()
        {
            return _builder.ToString();
        }
    }
}