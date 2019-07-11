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
            yield return new object[] { "Equal", boolValue, $"eq {boolValueString}" };
            yield return new object[] { "LessThan", boolValue, $"lt {boolValueString}" };
            yield return new object[] { "LessThanOrEqual", boolValue, $"le {boolValueString}" };
            yield return new object[] { "GreaterThan", boolValue, $"gt {boolValueString}" };
            yield return new object[] { "GreaterThanOrEqual", boolValue, $"ge {boolValueString}" };
            yield return new object[] { "NotEqual", boolValue, $"ne {boolValueString}" };

            // byte[]
            var binaryValue = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128, 255 };
            var binaryValueString = $"X'{string.Join("", binaryValue.Select(x => $"{x:x2}"))}'";
            yield return new object[] { "Equal", binaryValue, $"eq {binaryValueString}" };
            yield return new object[] { "LessThan", binaryValue, $"lt {binaryValueString}" };
            yield return new object[] { "LessThanOrEqual", binaryValue, $"le {binaryValueString}" };
            yield return new object[] { "GreaterThan", binaryValue, $"gt {binaryValueString}" };
            yield return new object[] { "GreaterThanOrEqual", binaryValue, $"ge {binaryValueString}" };
            yield return new object[] { "NotEqual", binaryValue, $"ne {binaryValueString}" };

            // date
            var dateValue = DateTimeOffset.Now;
            var dateValueString = $"datetime'{dateValue.ToUniversalTime():yyyy-MM-ddTHH:mm:ss.fffffff}Z'";
            yield return new object[] { "Equal", dateValue, $"eq {dateValueString}" };
            yield return new object[] { "LessThan", dateValue, $"lt {dateValueString}" };
            yield return new object[] { "LessThanOrEqual", dateValue, $"le {dateValueString}" };
            yield return new object[] { "GreaterThan", dateValue, $"gt {dateValueString}" };
            yield return new object[] { "GreaterThanOrEqual", dateValue, $"ge {dateValueString}" };
            yield return new object[] { "NotEqual", dateValue, $"ne {dateValueString}" };

            // double
            var doubleValue = double.MaxValue;
            var doubleValueString = doubleValue.ToString(CultureInfo.InvariantCulture);
            yield return new object[] { "Equal", doubleValue, $"eq {doubleValueString}" };
            yield return new object[] { "LessThan", doubleValue, $"lt {doubleValueString}" };
            yield return new object[] { "LessThanOrEqual", doubleValue, $"le {doubleValueString}" };
            yield return new object[] { "GreaterThan", doubleValue, $"gt {doubleValueString}" };
            yield return new object[] { "GreaterThanOrEqual", doubleValue, $"ge {doubleValueString}" };
            yield return new object[] { "NotEqual", doubleValue, $"ne {doubleValueString}" };

            // guid
            var guidValue = Guid.NewGuid();
            var guidValueString = $"guid'{guidValue}'";
            yield return new object[] { "Equal", guidValue, $"eq {guidValueString}" };
            yield return new object[] { "LessThan", guidValue, $"lt {guidValueString}" };
            yield return new object[] { "LessThanOrEqual", guidValue, $"le {guidValueString}" };
            yield return new object[] { "GreaterThan", guidValue, $"gt {guidValueString}" };
            yield return new object[] { "GreaterThanOrEqual", guidValue, $"ge {guidValueString}" };
            yield return new object[] { "NotEqual", guidValue, $"ne {guidValueString}" };

            // int
            var intValue = int.MaxValue;
            var intValueString = intValue.ToString();
            yield return new object[] { "Equal", intValue, $"eq {intValueString}" };
            yield return new object[] { "LessThan", intValue, $"lt {intValueString}" };
            yield return new object[] { "LessThanOrEqual", intValue, $"le {intValueString}" };
            yield return new object[] { "GreaterThan", intValue, $"gt {intValueString}" };
            yield return new object[] { "GreaterThanOrEqual", intValue, $"ge {intValueString}" };
            yield return new object[] { "NotEqual", intValue, $"ne {intValueString}" };

            // long
            var longValue = long.MaxValue;
            var longValueString = $"{longValue}L";
            yield return new object[] { "Equal", longValue, $"eq {longValueString}" };
            yield return new object[] { "LessThan", longValue, $"lt {longValueString}" };
            yield return new object[] { "LessThanOrEqual", longValue, $"le {longValueString}" };
            yield return new object[] { "GreaterThan", longValue, $"gt {longValueString}" };
            yield return new object[] { "GreaterThanOrEqual", longValue, $"ge {longValueString}" };
            yield return new object[] { "NotEqual", longValue, $"ne {longValueString}" };

            // string
            var stringValue = "xyz";
            var stringValueString = "'xyz'";
            yield return new object[] { "Equal", stringValue, $"eq {stringValueString}" };
            yield return new object[] { "LessThan", stringValue, $"lt {stringValueString}" };
            yield return new object[] { "LessThanOrEqual", stringValue, $"le {stringValueString}" };
            yield return new object[] { "GreaterThan", stringValue, $"gt {stringValueString}" };
            yield return new object[] { "GreaterThanOrEqual", stringValue, $"ge {stringValueString}" };
            yield return new object[] { "NotEqual", stringValue, $"ne {stringValueString}" };
        }

        [Fact]
        public void ShouldBuildStronglyTypedBoolFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.Bool).Equal(true);
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
            builder.Property(e => e.Bytes).Equal(value);
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
            builder.Property(e => e.Date).Equal(value);
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
            builder.Property(e => e.Double).Equal(value);
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
            builder.Property(e => e.Guid).Equal(value);
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
            builder.Property(e => e.Int).Equal(value);
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
            builder.Property(e => e.Long).Equal(value);
            builder
                .Build()
                .ShouldBe($"Long eq {valueString}");
        }

        [Fact]
        public void ShouldBuildStronglyTypedEqualFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).Equal("xyz");
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
        public void ShouldBuildStronglyTypedGreaterThanOrEqualFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).GreaterThanOrEqual("xyz");
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
        public void ShouldBuildStronglyTypedLessThanOrEqualFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).LessThanOrEqual("xyz");
            builder
                .Build()
                .ShouldBe("RowKey le 'xyz'");
        }

        [Fact]
        public void ShouldBuildStronglyTypedNotEqualFilter()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Property(e => e.RowKey).NotEqual("xyz");
            builder
                .Build()
                .ShouldBe("RowKey ne 'xyz'");
        }

        [Fact]
        public void ShouldCombineConditionsWithAnd()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();

            builder.And(x => x.Property("a").Equal(1));
            builder.Build().ShouldBe("a eq 1");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x => x.Property("a").Equal(1), x => x.Property("b").Equal(2));
            builder.Build().ShouldBe("(a eq 1) and (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x =>
            {
                x.Property("a").Equal(1);
                x.Property("b").Equal(2);
            });
            builder.Build().ShouldBe("(a eq 1) and (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x => x.Property("a").Equal(1), x => x.Property("b").Equal(2), x => x.Property("c").Equal(3));
            builder.Build().ShouldBe("((a eq 1) and (b eq 2)) and (c eq 3)");


            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(x =>
            {
                x.Property("a").Equal(1);
                x.Property("b").Equal(2);
                x.Property("c").Equal(3);
            });
            builder.Build().ShouldBe("((a eq 1) and (b eq 2)) and (c eq 3)");
        }

        [Fact]
        public void ShouldCombineConditionsWithOr()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();

            builder.Or(x => x.Property("a").Equal(1));
            builder.Build().ShouldBe("a eq 1");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x => x.Property("a").Equal(1), x => x.Property("b").Equal(2));
            builder.Build().ShouldBe("(a eq 1) or (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x =>
            {
                x.Property("a").Equal(1);
                x.Property("b").Equal(2);
            });
            builder.Build().ShouldBe("(a eq 1) or (b eq 2)");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x => x.Property("a").Equal(1), x => x.Property("b").Equal(2), x => x.Property("c").Equal(3));
            builder.Build().ShouldBe("((a eq 1) or (b eq 2)) or (c eq 3)");


            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(x =>
            {
                x.Property("a").Equal(1);
                x.Property("b").Equal(2);
                x.Property("c").Equal(3);
            });
            builder.Build().ShouldBe("((a eq 1) or (b eq 2)) or (c eq 3)");
        }

        [Fact]
        public void ShouldCombineConditionsUsingBothAndAndOr()
        {
            var builder = new TableQueryFilterBuilder<TestEntity>();
            builder.And(b1 =>
            {
                b1.Property("a").Equal(1);
                b1.Or(b2 =>
                {
                    b2.Property("b").Equal(2);
                    b2.And(b3 =>
                    {
                        b3.Property("c").Equal(3);
                        b3.Not(b4 => b4.Property("d").Equal(4));
                    });
                });
            });
            builder.Build().ShouldBe("(a eq 1) and ((b eq 2) or ((c eq 3) and (not (d eq 4))))");

            builder = new TableQueryFilterBuilder<TestEntity>();
            builder.Or(b1 =>
            {
                b1.Property("a").Equal(1);
                b1.And(b2 =>
                {
                    b2.Property("b").Equal(2);
                    b2.Or(b3 =>
                    {
                        b3.Property("c").Equal(3);
                        b3.Not(b4 => b4.Property("d").Equal(4));
                    });
                });
            });
            builder.Build().ShouldBe("(a eq 1) or ((b eq 2) and ((c eq 3) or (not (d eq 4))))");
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
