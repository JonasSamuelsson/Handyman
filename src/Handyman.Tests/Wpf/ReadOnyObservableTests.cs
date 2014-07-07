﻿using Handyman.Wpf;
using Shouldly;
using System.Collections.ObjectModel;
using System.Linq;

namespace Handyman.Tests.Wpf
{
    public class ReadOnyObservableTests
    {
        public void ShouldProvideValueFromSourceCollection()
        {
            var observable = ReadOnlyObservable.Create(new[]
            {
                new Observable<int>(1),
                new Observable<int>(2)
            },
                                                       x => x.Sum(y => y.Value));

            observable.Value.ShouldBe(3);
        }

        public void ChangesToTheUnderlyingObjectsShouldAffectValue()
        {
            var sourceObservable = new Observable<int>(1);
            var observable = ReadOnlyObservable.Create(new[]
            {
                sourceObservable,
                new Observable<int>(2)
            },
                                                       x => x.Sum(y => y.Value));
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(3);

            sourceObservable.Value = 2;

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(4);
        }

        public void ChangesToUnderlyingObservableCollecionShouldAffectValue()
        {
            var collection = new ObservableCollection<Observable<int>>(new[]
            {
                new Observable<int>(1),
                new Observable<int>(2)
            });
            var observable = ReadOnlyObservable.Create(collection, x => x.Sum(y => y.Value));
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(3);

            collection.Add(new Observable<int>(3));

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(6);
        }
    }
}