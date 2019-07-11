using System;
using System.Diagnostics;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    [DebuggerDisplay("{Build()}")]
    internal class TableQueryFilterNegateNode : ITableQueryFilterNode
    {
        private ITableQueryFilterNode _child;

        public void Add(ITableQueryFilterNode node)
        {
            if (_child != null)
                throw new InvalidOperationException();

            _child = node;
        }

        public string Build()
        {
            if (_child == null)
                throw new InvalidOperationException();

            return $"not ({_child.Build()})";
        }
    }
}