using Handyman.Wpf;
using Shouldly;
using System.Collections.ObjectModel;
using System.Linq;

namespace Handyman.Tests.Wpf
{
    public class ReadOnyObservableTests
    {
        public void ShouldProvideValueFromSource()
        {
            var observable = ReadOnlyObservable.Create(new[] { new Observable<int>(1), new Observable<int>(2) },
                                                       x => x.Sum(y => y.Value));

            observable.Value.ShouldBe(3);
        }

        public void ChangesToTheUnderlyingItemShouldAffectValue()
        {
            var sourceObservable = new Observable<int>(1);
            var observable = ReadOnlyObservable.Create(new[] { sourceObservable, new Observable<int>(2) },
                                                       x => x.Sum(y => y.Value));
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(3);

            sourceObservable.Value = 2;

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(4);
        }

        public void ChangesToUnderlyingCollectionShouldAffectValue()
        {
            var collection = new ObservableCollection<Observable<int>>(new[] { new Observable<int>(1), new Observable<int>(2) });
            var observable = ReadOnlyObservable.Create(collection, x => x.Sum(y => y.Value));
            var propertyChanged = false;
            observable.PropertyChanged += delegate { propertyChanged = true; };

            observable.Value.ShouldBe(3);

            collection.Add(new Observable<int>(3));

            propertyChanged.ShouldBe(true);
            observable.Value.ShouldBe(6);
        }

        public void ValidValueShouldNotResultInAnyValidationErrors()
        {
            var observable = ReadOnlyObservable.Create(new[] { new Observable<int>() },
                                                          x => x.Sum(y => y.Value));

            observable.Error.ShouldBe(string.Empty);
            observable["Value"].ShouldBe(string.Empty);
        }

        public void BrokenValidationRuleShouldResultInValidationError()
        {
            var errorMessage = "Value can't be zero";
            var observable = ReadOnlyObservable.Create(new[] { new Observable<int>() },
                                                       x => x.Sum(y => y.Value),
                                                       x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);
        }

        public void ShouldBeAbleToChangeUnderlyingItemsAfterConstruction()
        {
            var item1 = new Observable<int>(1);
            var item2 = new Observable<int>(2);
            var observable = ReadOnlyObservable.Create(new[] { item1 }, x => x.Sum(y => y.Value));


            var propertyChangedCount = 0;
            observable.PropertyChanged += delegate { propertyChangedCount++; };
            observable.Configure(x =>
            {
                x.Items.Remove(item1);
                x.Items.Add(item2);
            });

            propertyChangedCount.ShouldBe(1);
            observable.Value.ShouldBe(2);

            item1.Value = 1;
            propertyChangedCount.ShouldBe(1);
            observable.Value.ShouldBe(2);

            item2.Value = 1;
            propertyChangedCount.ShouldBe(2);
            observable.Value.ShouldBe(1);
        }

        public void ShouldBeAbleToChangeValueGetterAfterConstruction()
        {
            var observable = ReadOnlyObservable.Create(new[] { new Observable<double>(1), new Observable<double>(1) },
                                                       x => x.Sum(i => i.Value));

            observable.Configure(x => x.ValueGetter = y => y.Average(i => i.Value));

            observable.Value.ShouldBe(1);
        }

        public void ShouldBeAbleToAddValidationAfterConstruction()
        {
            var errorMessage = "Value can't be zero";
            var observable = ReadOnlyObservable.Create(new[] { new Observable<int>() },
                                               x => x.Sum(y => y.Value));
            observable.Configure(x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);
        }

        public void ShouldImplicitlyConvertToTValue()
        {
            var observable = ReadOnlyObservable.Create(new[] { new Observable<int>(5) }, x => x.Sum(y => y.Value));
            int value = observable;
            value.ShouldBe(observable.Value);
        }
    }
}