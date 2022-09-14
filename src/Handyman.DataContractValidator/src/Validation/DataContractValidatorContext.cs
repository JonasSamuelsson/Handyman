using Handyman.DataContractValidator.Model;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator.Validation
{
    internal class DataContractValidatorContext
    {
        private readonly List<string> _errors = new List<string>();
        private readonly Stack<string> _scopes = new Stack<string>();
        private readonly List<Validation> _validations = new List<Validation>();

        public IEnumerable<string> Errors => _errors;
        public DataContractValidatorOptions Options { get; set; }

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

        public void Validate(ITypeInfo actual, ITypeInfo expected)
        {
            actual = actual.GetValidatableTypeInfo();
            expected = expected.GetValidatableTypeInfo();

            if (_validations.Any(x => x.Actual == actual && x.Expected == expected))
            {
                return;
            }

            _validations.Add(new Validation
            {
                Actual = actual,
                Expected = expected
            });

            expected.GetValidator().Validate(actual, expected, this);
        }

        public void Validate(string scope, ITypeInfo actual, ITypeInfo expected)
        {
            _scopes.Push(scope);
            Validate(actual, expected);
            _scopes.Pop();
        }

        private class Validation
        {
            public ITypeInfo Actual { get; set; }
            public ITypeInfo Expected { get; set; }
        }
    }
}