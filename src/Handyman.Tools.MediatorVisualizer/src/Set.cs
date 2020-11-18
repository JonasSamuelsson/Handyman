using System;
using System.Collections.Generic;

namespace Handyman.Tools.MediatorVisualizer
{
    public class Set : SortedSet<string>
    {
        public static readonly Set Empty = new Set();

        public Set() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}