using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace Handyman.DuckTyping
{
    internal static class TypeGenerator
    {
        private static readonly string Namespace = $"{typeof(DuckTypedObject).Namespace}.GeneratedTypes";
        private static readonly Lazy<IEnumerable<Assembly>> DefaultAssemblies = new Lazy<IEnumerable<Assembly>>(GetDefaultAssemblies);

        private static int _counter = 1;

        private static IEnumerable<Assembly> GetDefaultAssemblies()
        {
            var assemblies = new HashSet<Assembly> { typeof(DuckTypedObject).Assembly };

            var dir = Path.GetDirectoryName(typeof(object).Assembly.Location);

            foreach (var dll in new[] { "mscorlib.dll", "System.dll", "System.Core.dll", "System.Runtime.dll" })
            {
                // these assemblies needs to be referenced manually, as per https://stackoverflow.com/a/47196516

                var path = Path.Combine(dir, dll);

                if (!File.Exists(path))
                    continue;

                var assembly = Assembly.LoadFile(path);

                assemblies.Add(assembly);
            }

            return assemblies;
        }

        internal static Type CreateClassImplementing(IEnumerable<Type> interfaces)
        {
            interfaces = interfaces as ICollection<Type> ?? interfaces.ToList();

            var name = GetName();
            var code = GenerateCodeForClassImplementing(name, interfaces);
            var references = GetReferencedAssemblies(@interfaces);
            return CompileClass(name, code, references);
        }

        private static string GetName()
        {
            return $"DuckTypedObject_{Interlocked.Increment(ref _counter)}";
        }

        private static string GenerateCodeForClassImplementing(string className, IEnumerable<Type> interfaces)
        {
            var dto = typeof(DuckTypedObject);
            interfaces = interfaces.OrderBy(x => x.FullName).ToList();

            var builder = new CodeBuilder();

            builder.AddLine($"namespace {Namespace}");

            using (builder.CreateScope())
            {
                builder.AddLine($"public class {className} : {dto.GetFullName()}, {string.Join(", ", interfaces.Select(x => x.GetFullName()))}");

                using (builder.CreateScope())
                {
                    foreach (var constructor in dto.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic))
                    {
                        if (constructor.IsPrivate)
                            continue;

                        var args = constructor.GetParameters();
                        var inputArgs = string.Join(", ", args.Select(x => $"{x.ParameterType.GetFullName()} {x.Name}"));
                        var outputArgs = string.Join(", ", args.Select(x => x.Name));

                        builder.AddLine($"public {className}({inputArgs}) : base({outputArgs}) {{ }}");
                    }

                    foreach (var @interface in interfaces)
                    {
                        foreach (var property in @interface.GetProperties())
                        {
                            var propertyType = property.PropertyType.GetFullName();
                            var propertyName = property.Name;

                            builder.AddLine();
                            builder.AddLine($"{propertyType} {@interface.GetFullName()}.{propertyName}");

                            using (builder.CreateScope())
                            {
                                builder.AddLine($"get => Get<{propertyType}>(\"{propertyName}\");");
                                builder.AddLine($"set => Set(\"{propertyName}\", value);");
                            }
                        }
                    }
                }
            }

            return builder.ToString();
        }

        private static IEnumerable<Assembly> GetReferencedAssemblies(IEnumerable<Type> interfaces)
        {
            var assemblies = new HashSet<Assembly>(DefaultAssemblies.Value);

            foreach (var @interface in interfaces)
            {
                assemblies.Add(@interface.Assembly);

                foreach (var property in @interface.GetProperties())
                {
                    assemblies.Add(property.PropertyType.Assembly);
                }
            }

            return assemblies;
        }

        private static Type CompileClass(string name, string code, IEnumerable<Assembly> references)
        {
            var assemblyName = $"{Namespace}.{name}";
            var syntaxTrees = new[] { CSharpSyntaxTree.ParseText(code) };
            var metadataReferences = references.Select(x => MetadataReference.CreateFromFile(x.Location));
            var compilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var compilation = CSharpCompilation.Create(assemblyName, syntaxTrees, metadataReferences, compilationOptions);

            using (var stream = new MemoryStream())
            {
                var result = compilation.Emit(stream);

                if (!result.Success)
                {
                    var message = string.Join(Environment.NewLine, result.Diagnostics.Select(x => x.ToString()));
                    throw new InvalidOperationException(message);
                }

                stream.Seek(0, SeekOrigin.Begin);

                var assembly = Assembly.Load(stream.ToArray());

                return assembly.GetTypes().Single();
            }
        }
    }
}