using System.Collections.Generic;
using Handyman.DataContractValidator.Model;

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
            return new DataContractReference { Resolve = () => _dataContracts[key] };
        }
    }
}