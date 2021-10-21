using System.Collections;

namespace Handyman.DataContractValidator
{
    public class Dictionary<TKey> : Hashtable
    {
        public Dictionary()
        {
        }

        public void Add(object valueDataContract)
        {
            base.Add(typeof(TKey), valueDataContract);
        }
    }
}