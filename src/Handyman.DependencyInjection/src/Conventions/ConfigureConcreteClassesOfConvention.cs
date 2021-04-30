using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.DependencyInjection.Conventions
{
    internal class ConfigureConcreteClassesOfConvention : IServiceConfigurationConvention
    {
        private static readonly ServiceConfigurationPolicy DefaultPolicy = ServiceConfigurationPolicy.TryAddEnumerable;
        private static readonly ServiceLifetime DefaultLifetime = ServiceLifetime.Transient;
        private static readonly IEnumerable<Type> EmptyTypes = new Type[] { };

        private readonly Type _type;
        private readonly ServiceConfigurationOptions _options;

        public ConfigureConcreteClassesOfConvention(Type type, ServiceConfigurationOptions options)
        {
            _type = type;
            _options = options;
        }

        public void Execute(IReadOnlyCollection<Type> types, IServiceCollection services)
        {
            var policy = _options.ConfigurationPolicy ?? DefaultPolicy;

            foreach (var type in types)
            {
                if (!IsConcreteClassOf(type, _type, out var baseTypes))
                    continue;

                var lifetime = ServiceLifetimeProvider.GetLifetimeOrNullFromAttribute(type) ?? _options.Lifetime ?? DefaultLifetime;

                foreach (var baseType in baseTypes)
                {
                    var serviceDescriptor = new ServiceDescriptor(baseType, type, lifetime);

                    ServiceConfigurationUtility
                        .Configure(services, serviceDescriptor, policy);
                }
            }
        }

        private static bool IsConcreteClassOf(Type candidate, Type typeToMatch, out IEnumerable<Type> matchingTypes)
        {
            matchingTypes = EmptyTypes;

            if (!candidate.IsConcreteClass())
                return false;

            var topCandidate = candidate;

            if (typeToMatch.IsClass)
            {
                var candidates = new List<Type>();

                while (candidate != null)
                {
                    candidates.Add(candidate);
                    candidate = candidate.BaseType;
                }

                return HasMatchingChildTypes(topCandidate, typeToMatch, candidates, ref matchingTypes);
            }

            if (typeToMatch.IsInterface)
            {
                var candidates = candidate.GetInterfaces().Distinct();

                return HasMatchingChildTypes(topCandidate, typeToMatch, candidates, ref matchingTypes);
            }

            return false;
        }

        private static bool HasMatchingChildTypes(Type type, Type typeToMatch, IEnumerable<Type> candidates, ref IEnumerable<Type> matchingTypes)
        {
            var matches = new List<Type>();

            if (type == typeToMatch)
                return false;

            if (type.IsGenericTypeDefinition && !typeToMatch.IsGenericTypeDefinition)
                return false;

            foreach (var candidate in candidates)
            {
                if (type.IsGenericTypeDefinition && !candidate.ContainsGenericParameters)
                    continue;

                if (!candidate.IsGenericTypeDefinition && !typeToMatch.IsGenericTypeDefinition)
                {
                    if (candidate != typeToMatch)
                        continue;

                    matches.Add(candidate);
                }

                else if (candidate.IsGenericTypeDefinition && typeToMatch.IsGenericTypeDefinition)
                {
                    if (candidate != typeToMatch)
                        continue;

                    matches.Add(candidate);
                }

                else if (typeToMatch.IsGenericTypeDefinition)
                {
                    if (!candidate.IsGenericType)
                        continue;

                    var candidateGenericTypeDefinition = candidate.GetGenericTypeDefinition();

                    if (candidateGenericTypeDefinition != typeToMatch)
                        continue;

                    matches.Add(candidate.ContainsGenericParameters ? candidateGenericTypeDefinition : candidate);
                }
            }

            if (matches.Count != 0)
            {
                matchingTypes = matches;
                return true;
            }

            return false;
        }
    }
}