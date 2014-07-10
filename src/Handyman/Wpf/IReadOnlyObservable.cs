using System.ComponentModel;

namespace Handyman.Wpf
{
    public interface IReadOnlyObservable<T> : INotifyPropertyChanged, IDataErrorInfo
    {
        T Value { get; }
    }
}