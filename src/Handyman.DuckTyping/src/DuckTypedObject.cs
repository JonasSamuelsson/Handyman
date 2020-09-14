using Handyman.DuckTyping.Serialization;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Dynamic;

namespace Handyman.DuckTyping
{
    public abstract class DuckTypedObject
    {
        protected DuckTypedObject() : this(DuckTyped.GetDefaultStorage())
        {
        }

        protected DuckTypedObject(object storage)
        {
            Initialize(GetDictionary(storage));
        }

        private void Initialize(object storage)
        {
            Dictionary = GetDictionary(storage);
            SerializationInfo.Initialize(this);
        }

        private static IDictionary<string, object> GetDictionary(object storage)
        {
            if (storage is ExpandoObject expando)
                return expando;

            if (storage is DuckTypedObject dto)
                return dto.Dictionary;

            throw new ArgumentException();
        }

        public static bool UseCaching { get; set; }

        internal IDictionary<string, object> Dictionary { get; private set; }

        protected T Get<T>(string property)
        {
            var type = typeof(T);

            if (DuckTyped.IsDuckTypedObject(type))
            {
                return GetDuckTypedObject<T>(property);
            }

            if (DuckTypedList.IsDuckTypedList(type))
            {
                return GetDuckTypedList<T>(property);
            }

            return GetValue<T>(property);
        }

        protected void Set<T>(string property, T value)
        {
            var type = typeof(T);

            if (DuckTyped.IsDuckTypedObject(type))
            {
                SetDuckTypedObject(property, value);
            }
            else if (DuckTypedList.IsDuckTypedList(type))
            {
                SetDuckTypedList(property, value);
            }
            else
            {
                SetValue(property, value);
            }
        }

        private void SetDuckTypedObject<T>(string property, T dto)
        {
            UpdateCache(property, dto);
            SetValue(property, ((DuckTypedObject)(object)dto).Dictionary);
        }

        private void SetDuckTypedList<T>(string property, T list)
        {
            UpdateCache(property, list);
            SetValue(property, ((DuckTypedList)(object)list).Dictionaries);
        }

        private void UpdateCache<T>(string property, T @object)
        {
            if (!UseCaching)
                return;

            var key = GetKey(property, typeof(T));
            SetValue(key, @object);
        }

        private void SetValue<T>(string key, T value)
        {
            Dictionary[key] = value;
        }

        private T GetDuckTypedObject<T>(string property)
        {
            if (UseCaching && TryGetCached<T>(property, out var item))
                return item;

            var dictionary = GetValue<IDictionary<string, object>>(property);

            if (dictionary == null)
                return default;

            var dto = DuckTyped.CreateDuckTypedObject<T>(dictionary);

            UpdateCache(property, dto);

            return dto;
        }

        private bool TryGetCached<T>(string property, out T item)
        {
            var key = GetKey(property, typeof(T));

            if (Dictionary.TryGetValue(key, out var o))
            {
                item = (T)o;
                return true;
            }

            item = default;
            return false;
        }

        private T GetDuckTypedList<T>(string property)
        {
            if (UseCaching && TryGetCached<T>(property, out var item))
                return item;

            var dictionaries = GetValue<List<IDictionary<string, object>>>(property);

            // todo this can probably be optimized using compiled expression
            var list = (T)Activator.CreateInstance(typeof(T), dictionaries);

            UpdateCache(property, list);

            return list;
        }

        private T GetValue<T>(string property)
        {
            if (Dictionary.TryGetValue(property, out var value))
                return (T)value;

            return default;
        }

        private static string GetKey(string property, Type type)
        {
            var @long = Utils.Combine(type.GetHashCode(), property.GetHashCode());
            return KeyCache.GetOrAdd(@long, _ => $"_cache:{@long}");
        }

        private static readonly ConcurrentDictionary<long, string> KeyCache = new ConcurrentDictionary<long, string>();
    }
}