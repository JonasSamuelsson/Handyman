using System;

namespace Handyman.DataContractValidator
{
    internal class DataContractResolver
    {
        private readonly Func<object> _func;

        public DataContractResolver(Func<object> func)
        {
            _func = func;
        }

        public object GetDataContract()
        {
            return _func.Invoke();
        }
    }
}