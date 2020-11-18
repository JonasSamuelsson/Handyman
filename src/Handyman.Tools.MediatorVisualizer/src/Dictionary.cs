using System;
using System.Collections.Generic;

namespace Handyman.Tools.MediatorVisualizer
{
    public class Dictionary : SortedDictionary<string, Set>
    {
        public Dictionary() : base(StringComparer.OrdinalIgnoreCase) { }
    }
}