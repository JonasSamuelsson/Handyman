using System;
using System.Diagnostics;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    [DebuggerDisplay("{Build()}")]
    internal class ConditionTableQueryFilterNode : ITableQueryFilterNode
    {
        private readonly string _condition;

        public ConditionTableQueryFilterNode(string condition)
        {
            _condition = condition;
        }

        public void Add(ITableQueryFilterNode node)
        {
            throw new NotSupportedException();
        }

        public string Build()
        {
            return _condition;
        }
    }
}