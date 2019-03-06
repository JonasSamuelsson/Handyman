using Handyman.DataContractValidator.Collections;
using Handyman.DataContractValidator.Dictionaries;
using Handyman.DataContractValidator.Enums;
using Handyman.DataContractValidator.Objects;
using Handyman.DataContractValidator.Values;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DataContractValidator
{
    internal class ValidationContext
    {
        private static readonly IEnumerable<IHandler> Handlers = new IHandler[]
        {
            new EnumHandler(),
            new ValueHandler(),
            new DictionaryHandler(),
            new CollectionHandler(),
            new ObjectHandler()
        };

        private readonly List<string> _scopes = new List<string>();

        internal readonly List<string> Errors = new List<string>();
        internal readonly FeatureCollection Features = new FeatureCollection();

        public string GetTypeName(object o)
        {
            if (o is IValidator validator)
                return validator.GetTypeName(this);

            foreach (var handler in Handlers)
            {
                if (!handler.TryGetTypeName(o, this, out var name)) continue;
                return name;
            }

            throw new InvalidOperationException();
        }

        public IReadOnlyCollection<string> Validate(Type type, object dataContract)
        {
            var validator = GetValidator(dataContract);
            validator.Validate(type, this);
            return Errors;
        }

        private static IValidator GetValidator(object dataContract)
        {
            if (dataContract is IValidator validator)
                return validator;

            foreach (var handler in Handlers)
            {
                if (!handler.TryGetValidator(dataContract, out validator)) continue;
                return validator;
            }

            throw new InvalidOperationException();
        }

        public IDisposable CreateScope(string name)
        {
            var index = _scopes.Count;
            _scopes.Add(name);
            return new Scope(() => _scopes.RemoveAt(index));
        }

        public void AddError(string message)
        {
            var prefix = _scopes.Any() ? $"{string.Join(".", _scopes)} : " : string.Empty;
            Errors.Add($"{prefix}{message}.");
        }

        private class Scope : IDisposable
        {
            private Action _action;

            public Scope(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                _action();
                _action = delegate { };
            }
        }

        internal class FeatureCollection
        {
            private readonly Dictionary<Type, object> _features = new Dictionary<Type, object>();

            public T GetOrAdd<T>() where T : new()
            {
                var type = typeof(T);

                if (!_features.TryGetValue(type, out var feature))
                {
                    feature = new T();
                    _features.Add(type, feature);
                }

                return (T)feature;
            }
        }
    }
}