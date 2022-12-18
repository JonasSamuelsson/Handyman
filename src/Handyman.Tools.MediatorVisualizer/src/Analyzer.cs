using Broslyn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FindSymbols;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Handyman.Tools.MediatorVisualizer
{
    public static class Analyzer
    {
        public static async Task<AnalyzerResult> Analyze(string path)
        {
            var project = LoadProject(path);
            var result = new AnalyzerResult();

            await GetHandlers(project, result);
            await GetDispatchers(project, result);
            GetEntryPoints(result);
            Trim(result);

            return result;
        }

        private static Project LoadProject(string path)
        {
            var workspace = CSharpCompilationCapture.Build(path).Workspace;
            return workspace.CurrentSolution.Projects.Single();
        }

        private static async Task GetHandlers(Project project, AnalyzerResult result)
        {
            var compilation = await project.GetCompilationAsync() ?? throw new InvalidOperationException();

            foreach (var syntaxTree in compilation.SyntaxTrees)
            {
                var model = compilation.GetSemanticModel(syntaxTree);

                foreach (var classDeclaration in syntaxTree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>())
                {
                    var namedTypeSymbol = model.GetDeclaredSymbol(classDeclaration);

                    if (namedTypeSymbol == null)
                        continue;

                    var configs = new[]
                    {
                        new
                        {
                            HandlerInterface = compilation.GetTypeByMetadataName("Handyman.Mediator.IEventHandler`1"),
                            MessageHandledBy = result.EventHandledBy
                        },
                        new
                        {
                            HandlerInterface = compilation.GetTypeByMetadataName("Handyman.Mediator.IRequestHandler`2"),
                            MessageHandledBy = result.RequestHandledBy
                        }
                    };

                    var genericInterfaces = namedTypeSymbol.AllInterfaces.Where(x => x.IsGenericType).ToList();

                    foreach (var config in configs)
                    {
                        foreach (var @interface in genericInterfaces)
                        {
                            if (@interface.ConstructedFrom.Equals(config.HandlerInterface, SymbolEqualityComparer.Default) == false)
                                continue;

                            var typeArgument = @interface.TypeArguments.First();

                            if (!(typeArgument is INamedTypeSymbol))
                                continue;

                            var handler = namedTypeSymbol.ToDisplayString();

                            result.Handlers.Add(handler);

                            var message = typeArgument.ToDisplayString();

                            if (config.MessageHandledBy.TryGetValue(message, out var messageHandledBy) == false)
                            {
                                messageHandledBy = config.MessageHandledBy[message] = new Set();
                            }

                            messageHandledBy.Add(handler);
                        }
                    }
                }
            }
        }

        private static async Task GetDispatchers(Project project, AnalyzerResult result)
        {
            var compilation = await project.GetCompilationAsync() ?? throw new InvalidOperationException();

            var configs = new[]
            {
                new
                {
                    DispatcherTypeNames = new[] { "IPublisher", "IPublisher`1" },
                    Method = "Publish",
                    HandlerDispatches = result.DispatcherPublishes
                },
                new
                {
                    DispatcherTypeNames = new[] { "ISender", "ISender`1", "Sender`2" },
                    Method = "Send",
                    HandlerDispatches = result.DispatcherSends
                }
            };

            foreach (var config in configs)
            {
                foreach (var dispatcherTypeName in config.DispatcherTypeNames)
                {
                    var dispatcherBaseType = compilation.GetTypeByMetadataName($"Handyman.Mediator.{dispatcherTypeName}");

                    if (dispatcherBaseType == null)
                        continue;

                    var dispatchMethod = dispatcherBaseType.GetMembers(config.Method).Single();
                    var dispatchReferences = await SymbolFinder.FindReferencesAsync(dispatchMethod, project.Solution);

                    foreach (var dispatchReference in dispatchReferences)
                    {
                        foreach (var location in dispatchReference.Locations)
                        {
                            var messageType = await GetMessageTypeOrNull(location);

                            if (messageType == null)
                                continue;

                            var dispatcherType = await GetDispatcherTypeOrNull(location, result.Handlers);

                            if (dispatcherType == null)
                                continue;

                            if (config.HandlerDispatches.TryGetValue(dispatcherType, out var handlerDispatches) == false)
                            {
                                config.HandlerDispatches[dispatcherType] = handlerDispatches = new Set();
                            }

                            handlerDispatches.Add(messageType);
                        }
                    }
                }
            }
        }

        private static async Task<string> GetMessageTypeOrNull(ReferenceLocation location)
        {
            var token = location.Location.SourceTree.GetRoot().FindToken(location.Location.SourceSpan.Start);

            for (var node = token.Parent; node != null; node = node.Parent)
            {
                if (!(node is InvocationExpressionSyntax invocationExpression))
                    continue;

                var argument = invocationExpression.ArgumentList.Arguments.First();

                var model = await location.Document.GetSemanticModelAsync();

                var typeInfo = model.GetTypeInfo(argument.Expression);

                if (typeInfo.Type?.TypeKind != TypeKind.Class)
                    return null;

                return typeInfo.Type.ToDisplayString();
            }

            return null;
        }

        private static async Task<string> GetDispatcherTypeOrNull(ReferenceLocation location, Set handlers)
        {
            var token = location.Location.SourceTree.GetRoot().FindToken(location.Location.SourceSpan.Start);

            IMethodSymbol methodSymbol = null;

            for (var node = token.Parent; node != null; node = node.Parent)
            {
                var model = await location.Document.GetSemanticModelAsync();

                if (node is MethodDeclarationSyntax methodDeclaration)
                    methodSymbol = model.GetDeclaredSymbol(methodDeclaration);

                if (!(node is ClassDeclarationSyntax classDeclaration))
                    continue;

                var namedTypeSymbol = model.GetDeclaredSymbol(classDeclaration);

                var dispatcher = namedTypeSymbol.ToDisplayString();

                if (handlers.Contains(dispatcher) == false)
                {
                    dispatcher += $".{methodSymbol.Name}";
                }

                return dispatcher;
            }

            return null;
        }

        private static void GetEntryPoints(AnalyzerResult result)
        {
            foreach (var publisher in result.DispatcherPublishes.Keys)
            {
                if (result.Handlers.Contains(publisher))
                    continue;

                result.EntryPoints.Add(publisher);
            }

            foreach (var sender in result.DispatcherSends.Keys)
            {
                if (result.Handlers.Contains(sender))
                    continue;

                result.EntryPoints.Add(sender);
            }
        }

        private static void Trim(AnalyzerResult result)
        {
            while (true)
            {
                var s = result.EntryPoints.First();

                var pattern = s.Substring(0, s.IndexOf('.') + 1);

                var isMatch = result.DispatcherPublishes.Keys
                    .Concat(result.DispatcherPublishes.SelectMany(x => x.Value))
                    .Concat(result.DispatcherSends.Keys)
                    .Concat(result.DispatcherSends.SelectMany(x => x.Value))
                    .Concat(result.EntryPoints)
                    .Concat(result.EventHandledBy.Keys)
                    .Concat(result.EventHandledBy.SelectMany(x => x.Value))
                    .Concat(result.Handlers)
                    .Concat(result.RequestHandledBy.Keys)
                    .Concat(result.RequestHandledBy.SelectMany(x => x.Value))
                    .All(x => x.StartsWith(pattern));

                if (isMatch == false)
                    return;

                Trim(result.DispatcherPublishes, pattern);
                Trim(result.DispatcherSends, pattern);
                Trim(result.EntryPoints, pattern);
                Trim(result.EventHandledBy, pattern);
                Trim(result.Handlers, pattern);
                Trim(result.RequestHandledBy, pattern);
            }
        }

        private static void Trim(Dictionary dictionary, string pattern)
        {
            foreach (var key in dictionary.Keys.ToList())
            {
                var trimmed = key.Substring(pattern.Length);
                var set = dictionary[key];
                dictionary.Add(trimmed, set);
                dictionary.Remove(key);
                Trim(set, pattern);
            }
        }

        private static void Trim(Set set, string pattern)
        {
            foreach (var s in set.ToList())
            {
                var trimmed = s.Substring(pattern.Length);
                set.Add(trimmed);
                set.Remove(s);
            }
        }
    }
}