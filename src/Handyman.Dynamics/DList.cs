using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Dynamics
{
    public class DList<T> : IEnumerable<T>
    {
        private readonly List<object> _list;

        public DList()
        {
            _list = new List<object>();
        }

        public DList(IEnumerable<T> source) : this()
        {
            foreach (var item in source) Add(item);
        }

        internal DList(List<object> list)
        {
            _list = list;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.Select(Utils.ConvertTo<T>).GetEnumerator();
        }

        public void Add(params object[] items)
        {
            foreach (var item in items) _list.Add(Convert(item));
        }

        public void Clear()
        {
            _list.Clear();
        }

        public void Insert(int index, object item)
        {
            _list.Insert(index, Convert(item));
        }

        public void Remove(int index)
        {
            _list.RemoveAt(index);
        }

        public int Remove(Func<T, bool> predicate)
        {
            return _list.RemoveAll(o => predicate((T)o));
        }

        private static object Convert(object o)
        {
            if (o == null || Utils.IsPrimitive(typeof(T)))
            {
                return Utils.ConvertTo<T>(o);
            }

            return DObject.Create(o);
        }
    }
}