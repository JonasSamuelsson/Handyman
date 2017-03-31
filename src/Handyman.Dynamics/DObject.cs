using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;

namespace Handyman.Dynamics
{
    [JsonConverter(typeof(DObjectConverter))]
    public class DObject
    {
        internal Dictionary<string, object> Dictionary { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        public object this[string key]
        {
            set { Set(key, value); }
        }

        public void Set(string key, object item)
        {
            Dictionary[key] = PrepareInput(item);
        }

        public void Add(string key, object item)
        {
            Dictionary.Add(key, PrepareInput(item));
        }

        public void Clear()
        {
            Dictionary.Clear();
        }

        public bool Contains(string key)
        {
            return Dictionary.ContainsKey(key);
        }

        public bool Remove(string key)
        {
            return Dictionary.Remove(key);
        }

        public void Rename(string fromKey, string toKey)
        {
            Add(toKey, Dictionary[fromKey]);
            Remove(fromKey);
        }

        public string GetString(string key)
        {
            return GetValue<string>(key);
        }

        public DList<string> GetStrings(string key)
        {
            var item = Dictionary[key];
            var list = (List<object>)item;
            return new DList<string>(list);
        }

        public T GetValue<T>(string key)
        {
            var value = Dictionary[key];
            return Utils.ConvertTo<T>(value);
        }

        public DList<T> GetValues<T>(string key)
        {
            var item = Dictionary[key];
            var list = (List<object>)item;
            return new DList<T>(list);
        }

        public DObject GetObject(string key)
        {
            return (DObject)Dictionary[key];
        }

        public DList<DObject> GetObjects(string key)
        {
            var item = Dictionary[key];
            var list = (List<object>)item;
            return new DList<DObject>(list);
        }

        public static DObject Create(object source)
        {
            if (source is DObject) return (DObject)source;

            var d = new DObject();

            var dictionary = source as IEnumerable<KeyValuePair<string, object>>;
            if (dictionary != null)
            {
                foreach (var kvp in dictionary) d.Set(kvp.Key, kvp.Value);
                return d;
            }

            if (source == null || source is IEnumerable || Utils.IsPrimitive(source))
                throw new InvalidCastException();

            foreach (var property in source.GetType().GetRuntimeProperties())
            {
                if (!property.CanRead) continue;
                var method = property.GetMethod;
                if (!method.IsPublic) continue;
                d.Set(property.Name, method.Invoke(source, null));
            }
            return d;
        }

        private static object PrepareInput(object input)
        {
            if (input == null || Utils.IsPrimitive(input)) return input;

            var enumerable = input as IEnumerable;
            if (enumerable != null)
            {
                var dictionary = input as IEnumerable<KeyValuePair<string, object>>;
                if (dictionary != null)
                {
                    return Create(dictionary);
                }

                return enumerable.OfType<object>().Select(PrepareInput).ToList();
            }

            return Create(input);
        }
    }
}
