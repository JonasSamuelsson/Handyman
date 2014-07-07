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
    }
}