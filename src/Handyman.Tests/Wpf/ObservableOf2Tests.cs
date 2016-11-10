using System;
using System.Collections.ObjectModel;
using System.Linq;
using Handyman.Extensions;
using Handyman.Wpf;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Wpf
{
    public class ObservableOf2Tests
    {
        [Fact]
        public void ShouldProvideValueFromSource()
        {
            var observable = Observable.Create(new[] { new Observable<double>(1), new Observable<double>(2) },
                                               x => x.Average(y => y.Value),
                                               (list, value) => { throw new NotImplementedException(); });

            observable.Value.ShouldBe(1.5);
        }

        [Fact]
        public void ChangesToTheUnderlyingItemShouldAffectValue()
        {
            var sourceObservable = new Observable<double>(1);
            var observable = Observable.Create(new[] { sourceObservable, new Observable<double>(2) },
                                               x => x.Average(y => y.Value),
                                               (list, value) => { throw new NotImplementedException(); });
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(1.5);

            sourceObservable.Value = 2;

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(2);
        }

        [Fact]
        public void ChangesToUnderlyingCollectionShouldAffectValue()
        {
            var collection = new ObservableCollection<Observable<double>>(new[] { new Observable<double>(1), new Observable<double>(2) });
            var observable = Observable.Create(collection,
                                               x => x.Average(y => y.Value),
                                               (list, value) => { throw new NotImplementedException(); });
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(1.5);

            collection.Add(new Observable<double>(3));

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(2);
        }

        [Fact]
        public void AssigningValueShouldInvokeValueSetter()
        {
            var observable = Observable.Create(new[] { new Observable<double>(1), new Observable<double>(2) },
                                               list => list.Average(x => x.Value),
                                               (list, value) => list.ForEach(x => x.Value = value));
            var propertyChangedCount = 0;
            observable.PropertyChanged += delegate { propertyChangedCount++; };

            observable.Value = 3;

            propertyChangedCount.ShouldBe(1);
            observable.Value.ShouldBe(3);
        }

        [Fact]
        public void ValidValueShouldNotResultInAnyValidationErrors()
        {
            var observable = Observable.Create(new[] { new Observable<int>() },
                                               x => x.Sum(y => y.Value),
                                               delegate { throw new NotImplementedException(); });

            observable.Error.ShouldBe(string.Empty);
            observable["Value"].ShouldBe(string.Empty);
        }

        [Fact]
        public void BrokenValidationRuleShouldResultInValidationError()
        {
            var errorMessage = "Value can't be zero";
            var observable = Observable.Create(new[] { new Observable<int>() },
                                               x => x.Sum(y => y.Value),
                                               delegate { throw new NotImplementedException(); },
                                               x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);
        }

        [Fact]
        public void ShouldBeAbleToAddValidationAfterConstruction()
        {
            var errorMessage = "Value can't be zero";
            var observable = Observable.Create(new[] { new Observable<int>() },
                                               x => x.Sum(y => y.Value),
                                               delegate { throw new NotImplementedException(); });
            observable.Configure(x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);
        }

        [Fact]
        public void ShouldImplicitlyConvertToTypeOfTValue()
        {
            var observable = Observable.Create(new[] { new Observable<int>(5) }, x => x.Sum(y => y.Value), delegate { });
            int value = observable;
            value.ShouldBe(observable.Value);
        }
    }
}