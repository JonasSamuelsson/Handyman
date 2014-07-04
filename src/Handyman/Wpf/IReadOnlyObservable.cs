using System.ComponentModel;

namespace Handyman.Wpf
{
    public interface IReadOnlyObservable<T> : INotifyPropertyChanged
    {
        T Value { get; }
    }
}