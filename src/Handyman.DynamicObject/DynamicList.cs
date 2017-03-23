using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DynamicObject
{
    public class DynamicList<T> : IEnumerable<T>
    {
        private readonly List<object> _list;

        internal DynamicList(List<object> list)
        {
            _list = list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Select(DynamicObject.ConvertTo<T>).GetEnumerator();
        }

        public void Add(params object[] items)
        {
            _list.AddRange(items.Select(DynamicObject.ConvertTo<T>).Cast<object>());
        }
    }
}