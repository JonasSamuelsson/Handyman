using Handyman.Azure.Cosmos.Table.Internals;
using Microsoft.Azure.Cosmos.Table;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Handyman.Azure.Cosmos.Table.Tests
{
    public class FilterBuilderTests
    {
        [Theory, MemberData(nameof(GetShouldBuildFilterParams))]
        public void ShouldBuildFilter(string operation, object value, string expected)
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();

            var conditionBuilder = builder.Property("Fake");
            var method = conditionBuilder.GetType().GetMethod(operation, new[] { value.GetType() });
            method.Invoke(conditionBuilder, new object[] { value });

            builder
                .Build()
                .ShouldBe($"Fake {expected}");
        }

        public static IEnumerable<object[]> GetShouldBuildFilterParams()
        {
            // bool
            var boolValue = true;
            var boolValueString = "true";
            yield return new object[] { "Equals", boolValue, $"eq {boolValueString}" };
            yield return new object[] { "LessThan", boolValue, $"lt {boolValueString}" };
            yield return new object[] { "LessThanOrEquals", boolValue, $"le {boolValueString}" };
            yield return new object[] { "GreaterThan", boolValue, $"gt {boolValueString}" };
            yield return new object[] { "GreaterThanOrEquals", boolValue, $"ge {boolValueString}" };
            yield return new object[] { "NotEquals", boolValue, $"ne {boolValueString}" };

            // byte[]
            var binaryValue = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 255 };
            var binaryValueString = $"X'{string.Join("", binaryValue.Select(x => $"{x:x2}"))}'";
            yield return new object[] { "Equals", binaryValue, $"eq {binaryValueString}" };
            yield return new object[] { "LessThan", binaryValue, $"lt {binaryValueString}" };
            yield return new object[] { "LessThanOrEquals", binaryValue, $"le {binaryValueString}" };
            yield return new object[] { "GreaterThan", binaryValue, $"gt {binaryValueString}" };
            yield return new object[] { "GreaterThanOrEquals", binaryValue, $"ge {binaryValueString}" };
            yield return new object[] { "NotEquals", binaryValue, $"ne {binaryValueString}" };

            // date
            var dateValue = DateTimeOffset.Now;
            var dateValueString = $"datetime'{dateValue.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffffff}Z'";
            yield return new object[] { "Equals", dateValue, $"eq {dateValueString}" };
            yield return new object[] { "LessThan", dateValue, $"lt {dateValueString}" };
            yield return new object[] { "LessThanOrEquals", dateValue, $"le {dateValueString}" };
            yield return new object[] { "GreaterThan", dateValue, $"gt {dateValueString}" };
            yield return new object[] { "GreaterThanOrEquals", dateValue, $"ge {dateValueString}" };
            yield return new object[] { "NotEquals", dateValue, $"ne {dateValueString}" };

            // double
            var doubleValue = double.MaxValue;
            var doubleValueString = doubleValue.ToString(CultureInfo.InvariantCulture);
            yield return new object[] { "Equals", doubleValue, $"eq {doubleValueString}" };
            yield return new object[] { "LessThan", doubleValue, $"lt {doubleValueString}" };
            yield return new object[] { "LessThanOrEquals", doubleValue, $"le {doubleValueString}" };
            yield return new object[] { "GreaterThan", doubleValue, $"gt {doubleValueString}" };
            yield return new object[] { "GreaterThanOrEquals", doubleValue, $"ge {doubleValueString}" };
            yield return new object[] { "NotEquals", doubleValue, $"ne {doubleValueString}" };

            // guid
            var guidValue = Guid.NewGuid();
            var guidValueString = $"guid'{guidValue}'";
            yield return new object[] { "Equals", guidValue, $"eq {guidValueString}" };
            yield return new object[] { "LessThan", guidValue, $"lt {guidValueString}" };
            yield return new object[] { "LessThanOrEquals", guidValue, $"le {guidValueString}" };
            yield return new object[] { "GreaterThan", guidValue, $"gt {guidValueString}" };
            yield return new object[] { "GreaterThanOrEquals", guidValue, $"ge {guidValueString}" };
            yield return new object[] { "NotEquals", guidValue, $"ne {guidValueString}" };

            // int
            var intValue = int.MaxValue;
            var intValueString = intValue.ToString();
            yield return new object[] { "Equals", intValue, $"eq {intValueString}" };
            yield return new object[] { "LessThan", intValue, $"lt {intValueString}" };
            yield return new object[] { "LessThanOrEquals", intValue, $"le {intValueString}" };
            yield return new object[] { "GreaterThan", intValue, $"gt {intValueString}" };
            yield return new object[] { "GreaterThanOrEquals", intValue, $"ge {intValueString}" };
            yield return new object[] { "NotEquals", intValue, $"ne {intValueString}" };

            // long
            var longValue = long.MaxValue;
            var longValueString = $"{longValue}L";
            yield return new object[] { "Equals", longValue, $"eq {longValueString}" };
            yield return new object[] { "LessThan", longValue, $"lt {longValueString}" };
            yield return new object[] { "LessThanOrEquals", longValue, $"le {longValueString}" };
            yield return new object[] { "GreaterThan", longValue, $"gt {longValueString}" };
            yield return new object[] { "GreaterThanOrEquals", longValue, $"ge {longValueString}" };
            yield return new object[] { "NotEquals", longValue, $"ne {longValueString}" };

            // string
            var stringValue = "xyz";
            var stringValueString = "'xyz'";
            yield return new object[] { "Equals", stringValue, $"eq {stringValueString}" };
            yield return new object[] { "LessThan", stringValue, $"lt {stringValueString}" };
            yield return new object[] { "LessThanOrEquals", stringValue, $"le {stringValueString}" };
            yield return new object[] { "GreaterThan", stringValue, $"gt {stringValueString}" };
            yield return new object[] { "GreaterThanOrEquals", stringValue, $"ge {stringValueString}" };
            yield return new object[] { "NotEquals", stringValue, $"ne {stringValueString}" };
        }

        [Fact]
        public void ShouldBuildStronglyTypedBoolFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Bool).Equals(true);
            builder
                .Build()
                .ShouldBe("Bool eq true");
        }

        [Fact]
        public void ShouldBuildStronglyTypedBytesFilter()
        {
            var value = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 255 };
            var valueString = $"X'{string.Join("", value.Select(x => $"{x:x2}"))}'";

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Bytes).Equals(value);
            builder
                .Build()
                .ShouldBe($"Bytes eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedDateFilter()
        {
            var value = DateTimeOffset.Now;
            var valueString = $"datetime'{value.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffffff}Z'";

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Date).Equals(value);
            builder
                .Build()
                .ShouldBe($"Date eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedDoubleFilter()
        {
            var value = double.MaxValue;
            var valueString = value.ToString(CultureInfo.InvariantCulture);

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Double).Equals(value);
            builder
                .Build()
                .ShouldBe($"Double eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedGuidFilter()
        {
            var value = Guid.NewGuid();
            var valueString = $"guid'{value}'";

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Guid).Equals(value);
            builder
                .Build()
                .ShouldBe($"Guid eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedIntFilter()
        {
            var value = int.MaxValue;
            var valueString = value.ToString();

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Int).Equals(value);
            builder
                .Build()
                .ShouldBe($"Int eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedLongFilter()
        {
            var value = long.MaxValue;
            var valueString = $"{value}L";

            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Long).Equals(value);
            builder
                .Build()
                .ShouldBe($"Long eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedEqualsFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).Equals("xyz");
            builder
                .Build()
                .ShouldBe("RowKey eq 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedGreaterThanFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).GreaterThan("xyz");
            builder
                .Build()
                .ShouldBe("RowKey gt 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedGreaterThanOrEqualsFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).GreaterThanOrEquals("xyz");
            builder
                .Build()
                .ShouldBe("RowKey ge 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedLessThanFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).LessThan("xyz");
            builder
                .Build()
                .ShouldBe("RowKey lt 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedLessThanOrEqualsFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).LessThanOrEquals("xyz");
            builder
                .Build()
                .ShouldBe("RowKey le 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedNotEqualsFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).NotEquals("xyz");
            builder
                .Build()
                .ShouldBe("RowKey ne 'xyz'");
        }

        [Fact]
        public void ShouldCombineConditionsWithAnd()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();

            builder.And(x => x.Property("a").Equals(1));
            builder.Build().ShouldBe("a eq 1");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x => x.Property("a").Equals(1), x => x.Property("b").Equals(2));
            builder.Build().ShouldBe("(a eq 1) and (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x =>
            {
                x.Property("a").Equals(1);
                x.Property("b").Equals(2);
            });
            builder.Build().ShouldBe("(a eq 1) and (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x => x.Property("a").Equals(1), x => x.Property("b").Equals(2), x => x.Property("c").Equals(3));
            builder.Build().ShouldBe("((a eq 1) and (b eq 2)) and (c eq 3)");


            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x =>
            {
                x.Property("a").Equals(1);
                x.Property("b").Equals(2);
                x.Property("c").Equals(3);
            });
            builder.Build().ShouldBe("((a eq 1) and (b eq 2)) and (c eq 3)");
        }

        [Fact]
        public void ShouldCombineConditionsWithOr()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();

            builder.Or(x => x.Property("a").Equals(1));
            builder.Build().ShouldBe("a eq 1");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x => x.Property("a").Equals(1), x => x.Property("b").Equals(2));
            builder.Build().ShouldBe("(a eq 1) or (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x =>
            {
                x.Property("a").Equals(1);
                x.Property("b").Equals(2);
            });
            builder.Build().ShouldBe("(a eq 1) or (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x => x.Property("a").Equals(1), x => x.Property("b").Equals(2), x => x.Property("c").Equals(3));
            builder.Build().ShouldBe("((a eq 1) or (b eq 2)) or (c eq 3)");


            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x =>
            {
                x.Property("a").Equals(1);
                x.Property("b").Equals(2);
                x.Property("c").Equals(3);
            });
            builder.Build().ShouldBe("((a eq 1) or (b eq 2)) or (c eq 3)");
        }

        [Fact]
        public void ShouldCombineConditionsUsingBothAndAndOr()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(b1 =>
            {
                b1.Property("a").Equals(1);
                b1.Or(b2 =>
                {
                    b2.Property("b").Equals(2);
                    b2.And(b3 =>
                    {
                        b3.Property("c").Equals(3);
                        b3.Not(b4 => b4.Property("d").Equals(4));
                    });
                });
            });
            builder.Build().ShouldBe("(a eq 1) and ((b eq 2) or ((c eq 3) and (not (d eq 4))))");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(b1 =>
            {
                b1.Property("a").Equals(1);
                b1.And(b2 =>
                {
                    b2.Property("b").Equals(2);
                    b2.Or(b3 =>
                    {
                        b3.Property("c").Equals(3);
                        b3.Not(b4 => b4.Property("d").Equals(4));
                    });
                });
            });
            builder.Build().ShouldBe("(a eq 1) or ((b eq 2) and ((c eq 3) or (not (d eq 4))))");
        }

        [Fact]
        public void ShouldBuildStringStartsWithFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property("RowKey").StartsWith("abc");
            builder.Build().ShouldBe("(RowKey ge 'abc') and (RowKey lt 'abd')");
        }

        [Fact]
        public void ShouldBuildStronglyTypedStringStartsWithFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(x => x.RowKey).StartsWith("abc");
            builder.Build().ShouldBe("(RowKey ge 'abc') and (RowKey lt 'abd')");
        }

        private class TestEntity : TableEntity
        {
            public bool Bool { get; set; }
            public byte[] Bytes { get; set; }
            public DateTimeOffset Date { get; set; }
            public double Double { get; set; }
            public Guid Guid { get; set; }
            public int Int { get; set; }
            public long Long { get; set; }
        }
    }
}
