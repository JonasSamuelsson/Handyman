using Handyman.DataContractValidator.Model;
using System;
using System.Collections.Generic;

namespace Handyman.DataContractValidator
{
    public class DataContractStore
    {
        private readonly Dictionary<object, object> _dataContracts = new Dictionary<object, object>();

        public void Add(object key, object dataContract)
        {
            _dataContracts.Add(key, dataContract);
        }

        public object Get(object key)
        {
            return new DataContractReference { Resolve = () => _dataContracts.TryGetValue(key, out var result) ? result : throw new InvalidOperationException($"{nameof(DataContractStore)} doesn't contain data contract with key '{key}'.") };
        }
    }
}