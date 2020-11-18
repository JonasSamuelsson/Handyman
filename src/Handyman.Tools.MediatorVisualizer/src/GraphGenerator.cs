using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Handyman.Tools.MediatorVisualizer
{
    public static class GraphGenerator
    {
        public static string GenerateGraph(string item, string layout, AnalyzerResult result)
        {
            var model = GetModel(item, result);

            var builder = new StringBuilder();

            builder.AppendLine("digraph {");

            builder.AppendLine($"  rankdir = {layout};");

            var boxes = model.DispatcherPublishes.Keys
               .Concat(model.DispatcherSends.Keys)
               .Concat(model.MessageHandledBy.SelectMany(x => x.Value))
               .Distinct();

            foreach (var box in boxes)
            {
                builder.AppendLine($"  \"{box}\" [shape=box]");
            }

            var events = model.DispatcherPublishes.SelectMany(x => x.Value)
               .Distinct();

            foreach (var @event in events)
            {
                builder.AppendLine($"  \"{@event}\" [shape=oval]");
            }

            var requests = model.DispatcherSends.SelectMany(x => x.Value)
               .Distinct();

            foreach (var request in requests)
            {
                builder.AppendLine($"  \"{request}\" [shape=hexagon]");
            }

            foreach (var kvp in model.MessageHandledBy)
            {
                foreach (var handledBy in kvp.Value)
                {
                    builder.AppendLine($"  \"{kvp.Key}\" -> \"{handledBy}\" [label=\"handled by\"]");
                }
            }

            foreach (var kvp in model.DispatcherPublishes)
            {
                foreach (var @event in kvp.Value)
                {
                    builder.AppendLine($"  \"{kvp.Key}\" -> \"{@event}\" [label=publish]");
                }
            }

            foreach (var kvp in model.DispatcherSends)
            {
                foreach (var @event in kvp.Value)
                {
                    builder.AppendLine($"  \"{kvp.Key}\" -> \"{@event}\" [label=send]");
                }
            }

            builder.AppendLine("}");

            return builder.ToString();
        }

        private static Model GetModel(string entryPoint, AnalyzerResult result)
        {
            var model = new Model();

            PopulateDispatcher(model, entryPoint, result);

            return model;
        }

        private static void PopulateDispatcher(Model model, string dispatcher, AnalyzerResult result)
        {
            if (model.Participants.Add(dispatcher) == false)
                return;

            foreach (var @event in result.DispatcherPublishes.GetValueOrDefault(dispatcher) ?? Set.Empty)
            {
                if (model.DispatcherPublishes.TryGetValue(dispatcher, out var publishes) == false)
                {
                    publishes = model.DispatcherPublishes[dispatcher] = new Set();
                }

                publishes.Add(@event);

                PopulateMessage(model, @event, result);
            }

            foreach (var request in result.DispatcherSends.GetValueOrDefault(dispatcher) ?? Set.Empty)
            {
                if (model.DispatcherSends.TryGetValue(dispatcher, out var sends) == false)
                {
                    sends = model.DispatcherSends[dispatcher] = new Set();
                }

                sends.Add(request);

                PopulateMessage(model, request, result);
            }
        }

        private static void PopulateMessage(Model model, string message, AnalyzerResult result)
        {
            if (model.Participants.Add(message) == false)
                return;

            foreach (var handler in result.EventHandledBy.GetValueOrDefault(message) ?? Set.Empty)
            {
                if (model.MessageHandledBy.TryGetValue(message, out var set) == false)
                {
                    set = model.MessageHandledBy[message] = new Set();
                }

                set.Add(handler);

                PopulateDispatcher(model, handler, result);
            }

            foreach (var handler in result.RequestHandledBy.GetValueOrDefault(message) ?? Set.Empty)
            {
                if (model.MessageHandledBy.TryGetValue(message, out var set) == false)
                {
                    set = model.MessageHandledBy[message] = new Set();
                }

                set.Add(handler);

                PopulateDispatcher(model, handler, result);
            }
        }

        private class Model
        {
            public Dictionary MessageHandledBy { get; } = new Dictionary();
            public Set Participants { get; } = new Set();
            public Dictionary DispatcherPublishes { get; } = new Dictionary();
            public Dictionary DispatcherSends { get; } = new Dictionary();
        }
    }
}