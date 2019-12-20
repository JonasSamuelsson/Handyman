using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Handyman.DependencyInjection
{
    internal class Scanner : IScanner
    {
        private readonly List<Type> _types = new List<Type>();
        private readonly List<Func<Type, bool>> _predicates = new List<Func<Type, bool>>();
        private readonly List<IServiceConfigurationConvention> _conventions = new List<IServiceConfigurationConvention>();

        public IScanner Types(IEnumerable<Type> types)
        {
            _types.AddRange(types);
            return this;
        }

        public IScanner Where(Func<Type, bool> filter)
        {
            _predicates.Add(filter);
            return this;
        }

        public IScanner Using(IServiceConfigurationConvention convention)
        {
            _conventions.Add(convention);
            return this;
        }

        internal void Execute(IServiceCollection services)
        {
            var types = _types
                .Distinct()
                .Where(x => !x.GetCustomAttributes<CompilerGeneratedAttribute>().Any())
                .Where(x => _predicates.All(y => y.Invoke(x)))
                .ToList();

            foreach (var convention in _conventions)
            {
                convention.Execute(types, services);
            }
        }
    }
}