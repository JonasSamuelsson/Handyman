using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;
using System;

namespace Handyman.Tools.Docs.Shared;

internal class TypeRegistrar : ITypeRegistrar
{
    private readonly IServiceCollection _services;

    public TypeRegistrar(IServiceCollection services)
    {
        _services = services;
    }

    public void Register(Type service, Type implementation)
    {
        _services.AddTransient(service, implementation);
    }

    public void RegisterInstance(Type service, object implementation)
    {
        _services.AddSingleton(service, implementation);
    }

    public void RegisterLazy(Type service, Func<object> factory)
    {
        _services.AddTransient(service, _ => factory());
    }

    public ITypeResolver Build()
    {
        return new TypeResolver(_services.BuildServiceProvider());
    }

    private class TypeResolver : ITypeResolver
    {
        private readonly IServiceProvider _serviceProvider;

        public TypeResolver(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public object Resolve(Type type)
        {
            return _serviceProvider.GetService(type);
        }
    }
}