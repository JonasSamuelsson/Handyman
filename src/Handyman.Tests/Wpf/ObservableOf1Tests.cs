using Handyman.Wpf;
using Shouldly;
using Xunit;

namespace Handyman.Tests.Wpf
{
    public class ObservableOf1Tests
    {
        [Fact]
        public void AssigningNewValueShouldTriggerPropertyChangedEvent()
        {
            var observable = new Observable<int>();
            var changedProperty = string.Empty;
            observable.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;
            observable.Value = 1;
            changedProperty.ShouldBe("Value");
        }

        [Fact]
        public void AssigningSameValueShouldTriggerPropertyChangedEvent()
        {
            var observable = new Observable<int>(1);
            var propertyChanged = false;
            observable.PropertyChanged += (sender, args) => propertyChanged = true;
            observable.Value = 1;
            propertyChanged.ShouldBe(false);
        }

        [Fact]
        public void ValidValueShouldNotResultInAnyValidationErrors()
        {
            var observable = new Observable<int>();

            observable.Error.ShouldBe(string.Empty);
            observable["Value"].ShouldBe(string.Empty);
        }

        [Fact]
        public void BrokenValidationRuleShouldResultInValidationError()
        {
            var errorMessage = "Value can't be zero.";
            var observable = new Observable<int>(0, x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);
        }

        [Fact]
        public void ShouldBeAbleToConfigureValidationAfterInstantiation()
        {
            var observable = new Observable<int>();

            var errorMessage = "Value can't be zero.";
            observable.Configure(x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
        }

        [Fact]
        public void ShouldImplicitlyConvertToTypeOfT()
        {
            var observable = new Observable<int>(5);
            int value = observable;
            value.ShouldBe(observable.Value);
        }
    }
}