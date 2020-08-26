using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DuckTyping
{
    public class DuckTypedList
    {
        internal List<IDictionary<string, object>> Dictionaries { get; private protected set; }

        internal static bool IsDuckTypedList(Type type)
        {
            while (type.BaseType != null)
            {
                type = type.BaseType;

                if (type == typeof(DuckTypedList))
                    return true;
            }

            return false;
        }
    }

    public class DuckTypedList<T> : DuckTypedList, IList<T>
        where T : DuckTypedObject
    {
        public DuckTypedList() : this(new List<IDictionary<string, object>>())
        {
        }

        // todo make this internal?
        public DuckTypedList(List<IDictionary<string, object>> dictionaries)
        {
            Dictionaries = dictionaries;
        }

        public T this[int index]
        {
            get => GetDto(Dictionaries[index]);
            set => Dictionaries[index] = GetDictionary(value);
        }

        public int Count => Dictionaries.Count;

        public bool IsReadOnly => false;

        public IEnumerator<T> GetEnumerator()
        {
            return Dictionaries.Select(GetDto).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(T item)
        {
            Dictionaries.Add(GetDictionary(item));
        }

        public void Clear()
        {
            Dictionaries.Clear();
        }

        public bool Contains(T item)
        {
            return Dictionaries.Contains(GetDictionary(item));
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(T item)
        {
            return Dictionaries.Remove(GetDictionary(item));
        }

        public int IndexOf(T item)
        {
            return Dictionaries.IndexOf(GetDictionary(item));
        }

        public void Insert(int index, T item)
        {
            Dictionaries.Insert(index, GetDictionary(item));
        }

        public void RemoveAt(int index)
        {
            Dictionaries.RemoveAt(index);
        }

        private static T GetDto(IDictionary<string, object> dictionary)
        {
            return (T)Activator.CreateInstance(typeof(T), dictionary);
        }

        private static IDictionary<string, object> GetDictionary(T item)
        {
            return item.Dictionary;
        }
    }
}