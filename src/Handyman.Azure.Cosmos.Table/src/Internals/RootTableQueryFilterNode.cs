﻿using System;
using System.Diagnostics;

namespace Handyman.Azure.Cosmos.Table.Internals
{
    [DebuggerDisplay("{Build()}")]
    internal class RootTableQueryFilterNode : ITableQueryFilterNode
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
            return _child?.Build() ?? throw new InvalidOperationException();
        }
    }
}