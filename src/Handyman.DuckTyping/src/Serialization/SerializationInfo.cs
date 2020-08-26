using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman.DuckTyping.Serialization
{
    internal static class SerializationInfo
    {
        private static readonly StringComparer Comparer = StringComparer.OrdinalIgnoreCase;
        private static readonly object SyncRoot = new object();
        private static readonly ReadOptimizedLookup<Type, int> TypeToIdLookup = new ReadOptimizedLookup<Type, int>();
        private static readonly ReadOptimizedLookup<long, int> LongToIdLookup = new ReadOptimizedLookup<long, int>();
        private static readonly ReadOptimizedLookup<int, HashSet<string>> IdToIgnoredPropertiesLookup = new ReadOptimizedLookup<int, HashSet<string>>();
        private static readonly HashSet<string> EmptySet = new HashSet<string>(Comparer);
        private static int NextId = 1;

        internal static void Initialize(DuckTypedObject dto)
        {
            if (dto == null) throw new ArgumentNullException(nameof(dto));
            if (dto.Dictionary == null) throw new ArgumentException();

            var type = dto.GetType();

            if (TryInitialize(dto, type))
                return;

            lock (SyncRoot)
            {
                if (TryInitialize(dto, type))
                    return;

                Initialize(type);
            }

            TryInitialize(dto, type);
        }

        private static void Initialize(Type type)
        {
            var id = 0;
            var ignoredProperties = GetIgnoredProperties(type);

            if (ignoredProperties.Any())
            {
                id = GetNextId();
                IdToIgnoredPropertiesLookup.Add(id, ignoredProperties);
            }

            TypeToIdLookup.Add(type, id);
        }

        private static int GetNextId() => NextId++;

        private static HashSet<string> GetIgnoredProperties(Type type)
        {
            var ignoredProperties = new HashSet<string>(Comparer);

            foreach (var property in type.GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                var attributes = property.GetCustomAttributes();

                if (attributes.All(x => x.GetType().Name.IndexOf("ignore", StringComparison.OrdinalIgnoreCase) == -1))
                    continue;

                ignoredProperties.Add(property.Name);
            }

            return ignoredProperties;
        }

        private static bool TryInitialize(DuckTypedObject dto, Type type)
        {
            if (!TypeToIdLookup.TryGetValue(type, out var id))
            {
                return false;
            }

            if (id != 0)
            {
                Initialize(dto.Dictionary, id);
            }

            return true;
        }

        private static readonly string DictionaryKey = "_ignore";

        private static void Initialize(IDictionary<string, object> dictionary, int id)
        {
            lock (dictionary)
            {
                if (dictionary.TryGetValue(DictionaryKey, out var value))
                {
                    var existingId = (int)value;

                    if (existingId != id)
                    {
                        id = Combine(existingId, id);
                    }
                }

                dictionary[DictionaryKey] = id;
            }
        }

        private static int Combine(int xId, int yId)
        {
            var key = Utils.Combine(xId, yId);

            if (LongToIdLookup.TryGetValue(key, out var id))
            {
                return id;
            }

            lock (SyncRoot)
            {
                if (LongToIdLookup.TryGetValue(key, out id))
                {
                    return id;
                }

                id = GetNextId();

                var properties = Combine(IdToIgnoredPropertiesLookup[xId], IdToIgnoredPropertiesLookup[yId]);

                IdToIgnoredPropertiesLookup.Add(id, properties);
                LongToIdLookup.Add(key, id);

                return id;
            }
        }

        private static HashSet<string> Combine(HashSet<string> x, HashSet<string> y)
        {
            var set = new HashSet<string>(Comparer);

            foreach (var s in x)
            {
                set.Add(s);
            }

            foreach (var s in y)
            {
                set.Add(s);
            }

            return set;
        }

        public static HashSet<string> GetPropertiesToIgnore(IDictionary<string, object> dictionary)
        {
            if (!dictionary.TryGetValue(DictionaryKey, out var value))
                return EmptySet;

            var id = (int)value;

            if (id == 0)
                return EmptySet;

            return IdToIgnoredPropertiesLookup[id];
        }
    }
}