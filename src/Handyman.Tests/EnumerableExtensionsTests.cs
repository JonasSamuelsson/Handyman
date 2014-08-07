using Shouldly;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Handyman.Tests
{
    public class EnumerableExtensionsTests
    {
        public void ForEachYieldShouldExecuteProvidedActionForEachItemWhenTheReturnedEnumerableIsTraversed()
        {
            var sum = 0;
            var ints = new[] { 1, 2, 3 };
            ints.ForEachYield(i => sum += i);
            sum.ShouldBe(0);
            ints.ForEachYield(i => sum += i).ToList();
            sum.ShouldBe(6);
        }

        public void ForEachShouldExecuteProvidedActionForEachItem()
        {
            var sum = 0;
            new[] { 1, 2, 3 }.ForEach(i => sum += i);
            sum.ShouldBe(6);
        }

        public void ShouldAppend()
        {
            new[] { 1, 2, 3 }.Append(4, 5, 6).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
            new[] { 1, 2, 3 }.Append(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 1, 2, 3, 4, 5, 6 });
        }

        public void ShouldPrepend()
        {
            new[] { 1, 2, 3 }.Prepend(4, 5, 6).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
            new[] { 1, 2, 3 }.Prepend(new List<int> { 4, 5, 6 }).ShouldBe(new[] { 4, 5, 6, 1, 2, 3 });
        }

        public void ShouldCheckIsEmpty()
        {
            var list = default(List<int>);
            Should.Throw<ArgumentNullException>(() => list.IsEmpty());

            list = new List<int>();
            list.IsEmpty().ShouldBe(true);

            list.Add(0);
            list.IsEmpty().ShouldBe(false);
        }

        public void ShouldCheckIsNullOrEmpty()
        {
            var list = default(List<int>);
            list.IsNullOrEmpty().ShouldBe(true);

            list = new List<int>();
            list.IsNullOrEmpty().ShouldBe(true);

            list.Add(0);
            list.IsNullOrEmpty().ShouldBe(false);
        }

        public void ShouldReturnDefaultIfSourceIsNull()
        {
            default(IEnumerable<int>).IfNull(new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
            default(IEnumerable<int>).IfNull(() => new[] { 1, 2, 3 }).ShouldBe(new[] { 1, 2, 3 });
        }

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

        public void ShouldEnumerateEnumerable()
        {
            var ints = new Ints(1, 2, 3);
            ints.Enumerate();
            ints.EnumerationCount.ShouldBe(1);
        }

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

        public void ShouldShuffle()
        {
            var numbers = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            var shuffled = numbers.Shuffle().ToArray();
            shuffled.ShouldNotBe(numbers);
            shuffled.OrderBy(x => x).ShouldBe(numbers);
        }

        public void ShouldCreateSetFromEnumerable()
        {
            var numbers = new[] { 1, 2, 3 };
            var set = numbers.ToSet();
            set.ShouldBeAssignableTo<ISet<int>>();
            set.ShouldBe(numbers);
        }

        public void ShouldCreateObservableColectionFromEnumerable()
        {
            var numbers = new[] { 1, 2, 3 };
            var set = numbers.ToObservableCollection();
            set.ShouldBeAssignableTo<ObservableCollection<int>>();
            set.ShouldBe(numbers);
        }

        public void ShouldSkipLast()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.SkipLast(2).ShouldBe(new[] { 1, 2, 3 });
        }

        public void ShouldSkipLastWhile()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.SkipLastWhile(i => i == 5).ShouldBe(new[] { 1, 2, 3, 4 });
        }

        public void ShouldTakeLast()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.TakeLast(2).ShouldBe(new[] { 4, 5 });
        }

        public void ShouldTakeLastWhile()
        {
            var ints = new[] { 1, 2, 3, 4, 5 };
            ints.TakeLastWhile(i => i == 5).ShouldBe(new[] { 5 });
        }
    }
}