using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using Handyman.Extensions;

namespace Handyman.Wpf
{
    public static class ReadOnlyObservable
    {
        public static ReadOnlyObservable<TItem, TValue> Create<TItem, TValue>(IEnumerable<TItem> items,
                                                                              Func<IReadOnlyList<TItem>, TValue> valueGetter,
                                                                              Action<ObservableValidationExpression<TValue>> configuration = null)
            where TItem : INotifyPropertyChanged
        {
            return new ReadOnlyObservable<TItem, TValue>(items, valueGetter, configuration ?? delegate { });
        }
    }

    public class ReadOnlyObservable<TItem, TValue> : IReadOnlyObservable<TValue>
        where TItem : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TItem> _items;
        private Func<IReadOnlyList<TItem>, TValue> _valueGetter;
        private TValue _value;
        private readonly List<Func<TValue, string>> _validators;
        private bool _ignoreCollectionChanged;

        internal ReadOnlyObservable(IEnumerable<TItem> items, Func<IReadOnlyList<TItem>, TValue> valueGetter, Action<ObservableValidationExpression<TValue>> configuration)
        {
            _items = items as ObservableCollection<TItem> ?? items.ToObservableCollection();
            _items.CollectionChanged += OnCollectionChanged;
            _items.ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            _valueGetter = valueGetter;
            _validators = new List<Func<TValue, string>>();

            if (configuration != null)
                configuration(new ConfigurationExpression { Validators = _validators });

            _value = _valueGetter(_items);
        }

        public void Configure(Action<ConfigurationExpression> configuration)
        {
            var expression = new ConfigurationExpression
            {
                Items = _items,
                Validators = _validators,
                ValueGetter = _valueGetter
            };
            using (new Temp(() => _ignoreCollectionChanged = true, () => _ignoreCollectionChanged = false))
                configuration(expression);

            _valueGetter = expression.ValueGetter;
            _value = _valueGetter(_items);
            OnPropertyChanged("Value");
        }

        public class ConfigurationExpression : ObservableValidationExpression<TValue>
        {
            public IList<TItem> Items { get; internal set; }
            public Func<IReadOnlyList<TItem>, TValue> ValueGetter { get; set; }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                args.OldItems.Cast<TItem>().ForEach(x => x.PropertyChanged -= OnItemPropertyChanged);
            if (args.NewItems != null)
                args.NewItems.Cast<TItem>().ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            if (_ignoreCollectionChanged) return;
            Value = _valueGetter(_items);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Value = _valueGetter(_items);
        }

        public TValue Value
        {
            get { return _value; }
            private set
            {
                if (EqualityComparer<TValue>.Default.Equals(value, _value)) return;
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
                return _validators.Select(x => x(_value))
                                  .Where(x => x.IsNotNullOrWhiteSpace())
                                  .Join(Environment.NewLine);
            }
        }

        public static implicit operator TValue(ReadOnlyObservable<TItem, TValue> observable)
        {
            return observable.Value;
        }
    }
}