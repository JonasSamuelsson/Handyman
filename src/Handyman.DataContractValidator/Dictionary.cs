using System.Collections;

namespace Handyman.DataContractValidator
{
    public class Dictionary<TKey> : Hashtable
    {
        public Dictionary(object valueDataContract)
            : base(new Hashtable { { typeof(TKey), valueDataContract } })
        {
        }
    }
}