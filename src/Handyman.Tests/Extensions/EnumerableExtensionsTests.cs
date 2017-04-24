using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Handyman.Extensions;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Extensions
{
    public class EnumerableExtensionsTests
    {
        [Fact]
        public void VisitShouldExecuteProvidedActionForEachItemWhenTheReturnedEnumerableIsTraversed()
        {
            var sum = 0;
            var ints = new[] { 1, 2, 3 };
            ints.Visit(i => sum += i);
            sum.ShouldBe(0);
            ints.Visit(i => sum += i).ToList();
            sum.ShouldBe(6);
        }

        [Fact]
        public void ForEachShouldExecuteProvidedActionForEachItem()
        {
            var sum = 0;
            new[] { 1, 2, 3 }.ForEach(i => sum += i);
            sum.ShouldBe(6);
        }

        [Fact]
        public void ShouldAppend()
        {
            new[] { 1, 2, 3 }.Append(4, 5, 6).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
            new[] { 1, 2, 3 }.Append(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
        }

        [Fact]
        public void ShouldPrepend()
        {
            new[] { 1, 2, 3 }.Prepend(4, 5, 6).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
            new[] { 1, 2, 3 }.Prepend(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
        }

        [Fact]
        public void ShouldCheckIsEmpty()
        {
            var list = default(List<int>);
            Should.Throw<ArgumentNullException>(() => list.IsEmpty());

            list = new List<int>();
            list.IsEmpty().ShouldBe(true);

            list.Add(0);
            list.IsEmpty().ShouldBe(false);
        }

        [Fact]
        public void ShouldCheckIsNullOrEmpty()
        {
            var list = default(List<int>);
            list.IsNullOrEmpty().ShouldBe(true);

            list = new List<int>();
            list.IsNullOrEmpty().ShouldBe(true);

            list.Add(0);
            list.IsNullOrEmpty().ShouldBe(false);
        }

        [Fact]
        public void ShouldReturnDefaultIfSourceIsNull()
        {
            default(IEnumerable<int>).IfNull(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
            default(IEnumerable<int>).IfNull(() => new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ShouldReturnDefaultIfSourceIsEmpty()
        {
            Enumerable.Empty<int>().IfEmpty(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
            Enumerable.Empty<int>().IfEmpty(() => new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });

            var ints = new Ints(1, 2, 3);
            ints.IfEmpty(new[] { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3 });
            ints.EnumerationCount.ShouldBe(1);
            ints = new Ints(1, 2, 3);
            ints.IfEmpty(new[] { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3 });
            ints.EnumerationCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldReturnDefaultIfSourceIsNullOrEmpty()
        {
            var ints = new Ints(1, 2, 3);
            ints.IfNullOrEmpty(new[] { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3 });
            ints.EnumerationCount.ShouldBe(1);
            ints = new Ints(1, 2, 3);
            ints.IfNullOrEmpty(() => new[] { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3 });
            ints.EnumerationCount.ShouldBe(1);

            default(IEnumerable<int>).IfNullOrEmpty(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
            default(IEnumerable<int>).IfNullOrEmpty(() => new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });

            Enumerable.Empty<int>().IfNullOrEmpty(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
            Enumerable.Empty<int>().IfNullOrEmpty(() => new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ShouldEnumerateEnumerable()
        {
            var ints = new Ints(1, 2, 3);
            ints.Enumerate();
            ints.EnumerationCount.ShouldBe(1);
        }

        [Fact]
        public void ShouldSumTimespans()
        {
            new[] { 1.Seconds(), 2.Seconds() }.Sum().ShouldBe(3.Seconds());
            new[] { 1, 2 }.Sum(x => x.Seconds()).ShouldBe(3.Seconds());
        }

        private class Ints : IEnumerable<int>
        {
            private List<int> _ints;

            public Ints() : this(new int[0]) { }

            public Ints(params int[] ints)
            {
                _ints = ints.ToList();
            }

            public int EnumerationCount { get; private set; }

            public IEnumerator<int> GetEnumerator()
            {
                EnumerationCount++;
                return _ints.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class Enumerable<T> : IEnumerable<T>
        {
            private readonly List<T> _elements;
            private bool _throwOnGetEnumerator;

            public Enumerable(params T[] elements)
            {
                _elements = elements.ToList();
            }

            public IEnumerator<T> GetEnumerator()
            {
                if (_throwOnGetEnumerator) throw new InvalidOperationException();
                _throwOnGetEnumerator = true;
                return _elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        [Fact]
        public void ShouldShuffle()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var shuffled = numbers.Shuffle().ToArray();
            shuffled.ShouldNotBe(numbers);
            shuffled.OrderBy(x => x).ShouldBe(numbers);
        }

        [Fact]
        public void ShouldCreateSetFromEnumerable()
        {
            var numbers = new[] { 1, 2, 3 };
            var set = numbers.ToSet();
            set.ShouldBeAssignableTo<ISet<int>>();
            set.ShouldBe(numbers);
        }

        [Fact]
        public void ShouldSkipLast()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.SkipLast(2).ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ShouldSkipLastWhile()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.SkipLastWhile(i => i == 5).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public void ShouldTakeLast()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.TakeLast(2).ShouldBe(new[] { 4, 5 });
        }

        [Fact]
        public void ShouldTakeLastWhile()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.TakeLastWhile(i => i == 5).ShouldBe(new[] { 5 });
        }

        [Fact]
        public void ShouldChunkToFixedChunkSize()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            var chunks = ints.Chunk(2).ToList();
            chunks.Count.ShouldBe(3);
            chunks[0].ShouldBe(new[] { 1, 2 });
            chunks[1].ShouldBe(new[] { 3, 4 });
            chunks[2].ShouldBe(new[] { 5 });
        }

        [Fact]
        public void ShouldChunkByPredicate()
        {
            var ints = new[] { 1, 1, 2, 1, 2, 3, 1, 2, 3, 4 };
            var chunks = ints.Chunk(i => i == 1).ToList();
            chunks.Count.ShouldBe(4);
            chunks[0].ShouldBe(new[] { 1 });
            chunks[1].ShouldBe(new[] { 1, 2 });
            chunks[2].ShouldBe(new[] { 1, 2, 3 });
            chunks[3].ShouldBe(new[] { 1, 2, 3, 4 });
        }

        [Fact]
        public void ForEachWithIndexShouldExecuteProvidedActionForEachItem()
        {
            var source = new[] { "zero", "one", "two" };
            var result = new List<string>();
            source.ForEach((s, i) => result.Add(s + i));
            result.Count.ShouldBe(3);
            result[0].ShouldBe("zero0");
            result[1].ShouldBe("one1");
            result[2].ShouldBe("two2");
        }

        [Fact]
        public void VisitWithIndexShouldExecuteProvidedActionForEachItem()
        {
            var source = new[] { "zero", "one", "two" };
            var result = new List<string>();

            source.Visit((s, i) => result.Add(s + i));
            result.Count.ShouldBe(0);

            source.Visit((s, i) => result.Add(s + i)).ToList();
            result.Count.ShouldBe(3);
            result[0].ShouldBe("zero0");
            result[1].ShouldBe("one1");
            result[2].ShouldBe("two2");
        }

        [Fact]
        public void ToReadOnlyCollection()
        {
            var ints = new[] { 1, 2, 3 };

            var readOnlyCollection = ints.ToReadOnlyCollection();

            readOnlyCollection.ShouldBeAssignableTo<IReadOnlyCollection<int>>();
            readOnlyCollection.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ToQueue()
        {
            var ints = new[] { 1, 2, 3 };

            var queue = ints.ToQueue();

            queue.ShouldBeOfType<Queue<int>>();
            queue.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ToStack()
        {
            var ints = new[] { 1, 2, 3 };

            var stack = ints.ToStack();

            stack.ShouldBeOfType<Stack<int>>();
            stack.ShouldBe(new[] { 3, 2, 1 });
        }

        [Fact]
        public void ToConcurrentBag()
        {
            var ints = new[] { 1, 2, 3 };

            var bag = ints.ToConcurrentBag();

            bag.ShouldBeOfType<ConcurrentBag<int>>();
            bag.OrderBy(i => i).ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ToConcurrentDictionary()
        {
            var ints = new[] { 1, 2, 3 };

            var dictionary = ints.ToConcurrentDictionary(i => i);

            dictionary.ShouldBeOfType<ConcurrentDictionary<int, int>>();
            ints.ForEach(i =>
            {
                int value;
                dictionary.TryGetValue(i, out value).ShouldBe(true);
                value.ShouldBe(i);
            });
        }

        [Fact]
        public void ToConcurrentDictionaryWithValueSelector()
        {
            var ints = new[] { 1, 2, 3 };

            var dictionary = ints.ToConcurrentDictionary(i => i, i => i.ToString());

            dictionary.ShouldBeOfType<ConcurrentDictionary<int, string>>();
            ints.ForEach(i =>
            {
                string value;
                dictionary.TryGetValue(i, out value).ShouldBe(true);
                value.ShouldBe(i.ToString());
            });
        }

        [Fact]
        public void ToConcurrentDictionaryShopuldThrowOnDuplicateKeys()
        {
            var ints = new[] { 1, 1 };

            Should.Throw<InvalidOperationException>(() => ints.ToConcurrentDictionary(i => i));
            Should.Throw<InvalidOperationException>(() => ints.ToConcurrentDictionary(i => i, i => i));
        }

        [Fact]
        public void ToConcurrentQueue()
        {
            var ints = new[] { 1, 2, 3 };

            var queue = ints.ToConcurrentQueue();

            queue.ShouldBeOfType<ConcurrentQueue<int>>();
            queue.ShouldBe(new[] { 1, 2, 3 });
        }

        [Fact]
        public void ToConcurrentStack()
        {
            var ints = new[] { 1, 2, 3 };

            var stack = ints.ToConcurrentStack();

            stack.ShouldBeOfType<ConcurrentStack<int>>();
            stack.ShouldBe(new[] { 3, 2, 1 });
        }

        [Fact]
        public void MinOrDefault()
        {
            var ints = new int[] { };

            ints.MinOrDefault().ShouldBe(0);
            ints.MinOrDefault(1).ShouldBe(1);
            ints.MinOrDefault(() => 2).ShouldBe(2);

            ints.MinOrDefault(i => i).ShouldBe(0);
            ints.MinOrDefault(i => i, 1).ShouldBe(1);
            ints.MinOrDefault(i => i, () => 2).ShouldBe(2);
        }
    }
}