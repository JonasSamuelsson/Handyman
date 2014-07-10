using Handyman.Wpf;
using Shouldly;

namespace Handyman.Tests.Wpf
{
    public class ObservableOf1Tests
    {
        public void AssigningNewValueShouldTriggerPropertyChangedEvent()
        {
            var observable = new Observable<int>();
            var changedProperty = string.Empty;
            observable.PropertyChanged += (sender, args) => changedProperty = args.PropertyName;
            observable.Value = 1;
            changedProperty.ShouldBe("Value");
        }

        public void AssigningSameValueShouldTriggerPropertyChangedEvent()
        {
            var observable = new Observable<int>(1);
            var propertyChanged = false;
            observable.PropertyChanged += (sender, args) => propertyChanged = true;
            observable.Value = 1;
            propertyChanged.ShouldBe(false);
        }

        public void ValidValueShouldNotResultInAnyValidationErrors()
        {
            var observable = new Observable<int>();

            observable.Error.ShouldBe(string.Empty);
            observable["Value"].ShouldBe(string.Empty);
        }

        public void BrokenValidationRuleShouldResultInValidationError()
        {
            var errorMessage = "Value can't be zero.";
            var observable = new Observable<int>(0, x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
            observable["Value"].ShouldBe(errorMessage);

            observable["value"].ShouldBe(string.Empty);
        }

        public void ShouldBeAbleToConfigureValidationAfterInstantiation()
        {
            var observable = new Observable<int>();

            var errorMessage = "Value can't be zero.";
            observable.Configure(x => x.Validators.Add(i => i == 0 ? errorMessage : string.Empty));

            observable.Error.ShouldBe(errorMessage);
        }
    }
}