using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.Mediator.Pipeline
{
    internal static class PipelineBuilderUtilities
    {
        private static readonly IReadOnlyList<Type> Empty = new List<Type>();

        public static void ApplyToggle<T>(List<T> list, IReadOnlyList<Type>? onEnabled, IReadOnlyList<Type>? onDisabled, bool toggleState)
        {
            if (toggleState)
            {
                Apply(list, onEnabled, onDisabled);
            }
            else
            {
                Apply(list, onDisabled, onEnabled);
            }
        }

        private static void Apply<T>(List<T> list, IReadOnlyList<Type>? enabled, IReadOnlyList<Type>? disabled)
        {
            enabled ??= Empty;
            disabled ??= Empty;

            if (enabled.Count + disabled.Count == 0)
                return;

            for (var i = list.Count - 1; i >= 0; i--)
            {
                var item = list[i];
                var type = item!.GetType();

                if (enabled.Contains(type))
                    continue;

                if (!disabled.Contains(type))
                    continue;

                list.RemoveAt(i);
            }
        }
    }
}