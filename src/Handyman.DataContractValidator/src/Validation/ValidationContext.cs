using Handyman.DataContractValidator.Model;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Validation
{
    internal class ValidationContext
    {
        private readonly List<string> _errors = new List<string>();
        private readonly Stack<string> _scopes = new Stack<string>();
        private readonly ITypeInfoValidator[] _validators =
        {
            new AnyValidator(),
            new EnumValidator(),
            new ValueValidator(),
            new DictionaryValidator(),
            new CollectionValidator(),
            new ObjectValidator()
        };

        public IEnumerable<string> Errors => _errors;

        public void AddError(string error)
        {
            var prefix = _scopes.Any() ? $"{string.Join(".", _scopes.Reverse())} : " : "";
            _errors.Add($"{prefix}{error}");
        }

        public void AddError(string scope, string error)
        {
            _scopes.Push(scope);
            AddError(error);
            _scopes.Pop();
        }

        public void Validate(TypeInfo actual, TypeInfo expected)
        {
            expected.GetValidator().Validate(actual, expected, this);
        }

        public void Validate(string scope, TypeInfo actual, TypeInfo expected)
        {
            _scopes.Push(scope);
            Validate(actual, expected);
            _scopes.Pop();
        }
    }
}