using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.CompilerServices;

namespace Handyman.DuckTyping.Benchmarks
{
    //[DryJob]
    [MemoryDiagnoser]
    public class Program
    {
        public static void Main()
        {
            BenchmarkRunner.Run<Program>();
        }

        public static int Iterations { get; set; } = 1_000;

        [Benchmark]
        public void DynamicObjectCreate()
        {
            Execute(() => NoOp(new ExpandoObject()));
        }

        [Benchmark]
        public void DynamicObjectGetValue()
        {
            dynamic target = new ExpandoObject();
            target.Value = string.Empty;

            Execute(() => NoOp(target.Value));
        }

        [Benchmark]
        public void DynamicObjectSetList()
        {
            dynamic target = new ExpandoObject();
            var list = new List<ExpandoObject>();

            Execute(() => target.List = list);
        }

        [Benchmark]
        public void DynamicObjectSetObject()
        {
            dynamic target = new ExpandoObject();
            var @object = new ExpandoObject();

            Execute(() => target.Object = @object);
        }

        [Benchmark]
        public void DynamicObjectSetValue()
        {
            dynamic target = new ExpandoObject();

            Execute(() => target.Value = string.Empty);
        }

        [Benchmark]
        public void RegularListAdd()
        {
            var list = new List<RegularTarget>();
            var item = new RegularTarget();

            Execute(() => list.Add(item));
        }

        [Benchmark]
        public void RegularListCreate()
        {
            Execute(() => NoOp(new List<RegularTarget>()));
        }

        [Benchmark]
        [Arguments(1)]
        [Arguments(5)]
        [Arguments(10)]
        [Arguments(20)]
        [Arguments(50)]
        [Arguments(100)]
        public void RegularListEnumerate(int itemCount)
        {
            var list = new List<RegularTarget>();
            var item = new RegularTarget();

            for (var i = 0; i < itemCount; i++)
            {
                list.Add(item);
            }

            Execute(() => NoOp(list));
        }

        [Benchmark]
        public void RegularObjectCreate()
        {
            Execute(() => NoOp(new RegularTarget()));
        }

        [Benchmark]
        public void RegularObjectGetValue()
        {
            var target = new RegularTarget { Value = string.Empty };

            Execute(() => NoOp(target.Value));
        }

        [Benchmark]
        public void RegularObjectSetObject()
        {
            var target = new RegularTarget();
            var @object = new RegularTarget();

            Execute(() => target.Object = @object);
        }

        //[Benchmark]
        //public void SourceGeneratorListAdd()
        //{
        //    var list = new DuckTypedList<SourceGeneratorObject> { Expandos = new List<ExpandoObject>() };
        //    var item = new SourceGeneratorObject();

        //    Execute(() => list.Add(item));
        //}

        //[Benchmark]
        //public void SourceGeneratorListCreate()
        //{
        //    Execute(() => new DuckTypedList<SourceGeneratorObject> { Expandos = new List<ExpandoObject>() });
        //}

        //[Benchmark]
        //[Arguments(1, false)]
        //[Arguments(5, false)]
        //[Arguments(10, false)]
        //[Arguments(20, false)]
        //[Arguments(50, false)]
        //[Arguments(100, false)]
        //[Arguments(1, true)]
        //[Arguments(5, true)]
        //[Arguments(10, true)]
        //[Arguments(20, true)]
        //[Arguments(50, true)]
        //[Arguments(100, true)]
        //public void SourceGeneratorListEnumerate(int itemCount, bool useCaching)
        //{
        //    DuckTypedObject.UseCaching = useCaching;

        //    var list = new DuckTypedList<SourceGeneratorObject> { Expandos = new List<ExpandoObject>() };
        //    var item = new SourceGeneratorObject();

        //    for (var i = 0; i < itemCount; i++)
        //    {
        //        list.Add(item);
        //    }

        //    Execute(() => NoOp(list));
        //}

        [Benchmark]
        public void SourceGeneratorObjectCreate()
        {
            Execute(() => NoOp(new SourceGeneratorObject()));
        }

        // SourceGeneratorObjectGetListWithCaching
        // SourceGeneratorObjectGetListWithoutCaching

        [Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public void SourceGeneratorObjectGetObject(bool useCaching)
        {
            DuckTypedObject.UseCaching = useCaching;

            var target = new SourceGeneratorObject();
            target.Object = new SourceGeneratorObject();

            Execute(() => NoOp(target.Object));
        }

        [Benchmark]
        public void SourceGeneratorObjectGetValue()
        {
            var target = new SourceGeneratorObject();
            target.Value = string.Empty;

            Execute(() => NoOp(target.Value));
        }

        // SourceGeneratorObjectSetListWithCaching
        // SourceGeneratorObjectSetListWithoutCaching

        [Benchmark]
        [Arguments(false)]
        [Arguments(true)]
        public void SourceGeneratorObjectSetObject(bool useCaching)
        {
            DuckTypedObject.UseCaching = useCaching;

            var target = new SourceGeneratorObject();
            var @object = new SourceGeneratorObject();

            Execute(() => target.Object = @object);
        }

        [Benchmark]
        public void SourceGeneratorObjectSetValue()
        {
            var target = new SourceGeneratorObject();
            var value = string.Empty;

            Execute(() => target.Value = value);
        }

        private static void Execute(Action action)
        {
            for (var i = 0; i < Iterations; i++)
            {
                action.Invoke();
            }
        }

        public static long Counter;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void NoOp(object _) => Counter += 1;

        [MethodImpl(MethodImplOptions.NoInlining)]
        private static void NoOp<T>(IEnumerable<T> enumerable)
        {
            foreach (var _ in enumerable) NoOp(_);
        }

        public class SourceGeneratorObject : DuckTypedObject
        {
            public DuckTypedList<SourceGeneratorObject> List
            {
                get => Get<DuckTypedList<SourceGeneratorObject>>(nameof(List));
                set => Set(nameof(List), value);
            }

            public SourceGeneratorObject Object
            {
                get => Get<SourceGeneratorObject>(nameof(Object));
                set => Set(nameof(Object), value);
            }

            public string Value
            {
                get => Get<string>(nameof(Value));
                set => Set(nameof(Value), value);
            }
        }

        public class RegularTarget
        {
            public List<RegularTarget> List { get; set; }
            public RegularTarget Object { get; set; }
            public string Value { get; set; }
        }

        public interface ITarget
        {
            IList<ITarget> List { get; set; }
            ITarget Object { get; set; }
            string Value { get; set; }
        }
    }
}
