﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    [DebuggerDisplay("{Build()}")]
    internal abstract class CompositionTableQueryFilterNode : ITableQueryFilterNode
    {
        private readonly string _operator;
        private readonly List<ITableQueryFilterNode> _nodes = new List<ITableQueryFilterNode>();

        protected CompositionTableQueryFilterNode(string @operator)
        {
            _operator = @operator;
        }

        public void Add(ITableQueryFilterNode node)
        {
            _nodes.Add(node);
        }

        public string Build()
        {
            if (_nodes.Count == 0)
                throw new InvalidOperationException();

            var result = _nodes[0].Build();

            for (var i = 1; i < _nodes.Count; i++)
            {
                var node = _nodes[i];
                var filter = node.Build();
                result = FilterConditionGenerator.CombineFilters(result, _operator, filter);
            }

            return result;
        }
    }
}