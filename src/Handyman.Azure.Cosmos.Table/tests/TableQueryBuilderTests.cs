using Shouldly;
using System;
using Xunit;

namespace Handyman.Azure.Cosmos.Table.Tests
{
    public class TableQueryBuilderTests
    {
        [Fact]
        public void ShouldThrowOnMultipleWhereCalls()
        {
            var tqb = new TableQueryBuilder();

            tqb.Where(x => x.PartitionKey.StartsWith("1"));

            Should.Throw<Exception>(() => tqb.Where(x => x.RowKey.StartsWith("2")));
        }

        [Fact]
        public void ShouldNotThrowOnMultipleSelectCalls()
        {
            var tqb = new TableQueryBuilder();

            tqb.Select(x => x.PartitionKey());

            Should.Throw<Exception>(() => tqb.Select(x => x.RowKey()));
        }
    }
}