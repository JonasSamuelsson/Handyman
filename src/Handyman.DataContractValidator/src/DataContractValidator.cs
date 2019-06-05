using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    public class DataContractValidator
    {
        private readonly Dictionary<string, object> _dataContracts = new Dictionary<string, object>();

        public void AddDataContract(string key, object dataContract)
        {
            _dataContracts.Add(key, dataContract);
        }

        public object GetDataContract(string key)
        {
            return new DataContractResolver(() => _dataContracts[key]);
        }

        public void Validate(Type type, object dataContract)
        {
            if (Validate(type, dataContract, out var errors))
                return;

            throw new ValidationException(string.Join(Environment.NewLine, errors));
        }

        public bool Validate(Type type, object dataContract, out IEnumerable<string> errors)
        {
            var context = new ValidationContext();

            errors = context.Validate(type, dataContract);

            return !errors.Any();
        }
    }
}
