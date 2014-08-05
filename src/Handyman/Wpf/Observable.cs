using Handyman.Annotations;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Handyman.Wpf
{
    public class Observable<T> : IObservable<T>
    {
        private T _value;
        private readonly List<Func<T, string>> _validators = new List<Func<T, string>>();

        public Observable(T value = default (T))
            : this(value, delegate { })
        {
        }

        public Observable(T value, Action<ObservableValidationExpression<T>> configuration)
        {
            _value = value;
            Configure(configuration);
        }

        public T Value
        {
            get { return _value; }
            set
            {
                if (EqualityComparer<T>.Default.Equals(value, _value)) return;
                _value = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get { return columnName == "Value" ? Error : string.Empty; }
        }

        public string Error
        {
            get
            {
                return _validators.Select(validator => validator(_value))
                                  .Where(x => x.IsNotNullOrWhiteSpace())
                                  .Join(Environment.NewLine);
            }
        }

        public void Configure(Action<ObservableValidationExpression<T>> configuration)
        {
            var expression = new ObservableValidationExpression<T> { Validators = _validators };
            configuration(expression);
            OnPropertyChanged("Value");
        }

        public static implicit operator T(Observable<T> observable)
        {
            return observable.Value;
        }
    }

    public class Observable<TItem, TValue> : IObservable<TValue>
        where TItem : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TItem> _collection;
        private readonly Func<IReadOnlyList<TItem>, TValue> _valueGetter;
        private readonly Action<IList<TItem>, TValue> _valueSetter;
        private TValue _value;
        private bool _isValueChanging;
        private readonly List<Func<TValue, string>> _validators;

        internal Observable(IEnumerable<TItem> items, Func<IReadOnlyList<TItem>, TValue> valueGetter, Action<IList<TItem>, TValue> valueSetter, Action<ObservableValidationExpression<TValue>> configuration)
        {
            _collection = items as ObservableCollection<TItem> ?? items.ToObservableCollection();
            _collection.CollectionChanged += OnCollectionChanged;
            _collection.ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            _valueGetter = valueGetter;
            _valueSetter = valueSetter;
            _value = _valueGetter(_collection);
            _validators = new List<Func<TValue, string>>();
            Configure(configuration);
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                args.OldItems.Cast<TItem>().ForEach(x => x.PropertyChanged -= OnItemPropertyChanged);
            if (args.NewItems != null)
                args.NewItems.Cast<TItem>().ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            if (_isValueChanging) return;
            var value = _valueGetter(_collection);
            SetValue(value, Cascade.No);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_isValueChanging) return;
            var value = _valueGetter(_collection);
            SetValue(value, Cascade.No);
        }

        public TValue Value
        {
            get { return _value; }
            set { SetValue(value, Cascade.Yes); }
        }

        private void SetValue(TValue value, Cascade cascade)
        {
            using (new Temp(() => _isValueChanging = true, () => _isValueChanging = false))
            {
                if (EqualityComparer<TValue>.Default.Equals(value, _value)) return;
                if (cascade == Cascade.Yes) _valueSetter(_collection, value);
                _value = value;
                OnPropertyChanged();
            }
        }

        private enum Cascade
        {
            Yes,
            No
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        public string this[string columnName]
        {
            get { return columnName == "Value" ? Error : string.Empty; }
        }

        public string Error
        {
            get
            {
                return _validators.Select(x => x(_value))
                                  .Where(x => x.IsNotNullOrWhiteSpace())
                                  .Join(Environment.NewLine);
            }
        }

        public void Configure(Action<ObservableValidationExpression<TValue>> configuration)
        {
            var expression = new ObservableValidationExpression<TValue>
            {
                Validators = _validators
            };
            configuration(expression);
            OnPropertyChanged("Value");
        }

        public static implicit operator TValue(Observable<TItem, TValue> observable)
        {
            return observable.Value;
        }
    }

    public static class Observable
    {
        public static Observable<TItem, TValue> Create<TItem, TValue>(IEnumerable<TItem> items,
                                                                      Func<IReadOnlyList<TItem>, TValue> valueGetter,
                                                                      Action<IList<TItem>, TValue> valueSetter,
                                                                      Action<ObservableValidationExpression<TValue>>
                                                                          configuration = null)
            where TItem : INotifyPropertyChanged
        {
            return new Observable<TItem, TValue>(items, valueGetter, valueSetter, configuration ?? delegate { });
        }
    }
}