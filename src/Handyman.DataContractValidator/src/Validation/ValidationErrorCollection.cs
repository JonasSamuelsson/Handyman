using System.Collections;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Validation
{
    internal class ValidationErrorCollection : IEnumerable<string>
    {
        private readonly List<string> _errors = new List<string>();

        public void Add(string error)
        {
            _errors.Add(error);
        }

        public IEnumerator<string> GetEnumerator() => _errors.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}