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
    public static class ReadOnlyObservable
    {
        public static ReadOnlyObservable<TItem, TValue> Create<TItem, TValue>(IEnumerable<TItem> items,
                                                                              Func<IReadOnlyList<TItem>, TValue> valueProvider,
                                                                              Action<ObservableValidationExpression<TValue>> configuration = null)
            where TItem : INotifyPropertyChanged
        {
            return new ReadOnlyObservable<TItem, TValue>(items, valueProvider, configuration ?? delegate { });
        }
    }

    public class ReadOnlyObservable<TItem, TValue> : IReadOnlyObservable<TValue>
        where TItem : INotifyPropertyChanged
    {
        private readonly ObservableCollection<TItem> _collection;
        private readonly Func<IReadOnlyList<TItem>, TValue> _valueProvider;
        private TValue _value;
        private readonly List<Func<TValue, string>> _validators;

        internal ReadOnlyObservable(IEnumerable<TItem> items, Func<IReadOnlyList<TItem>, TValue> valueProvider, Action<ObservableValidationExpression<TValue>> configuration)
        {
            _collection = items as ObservableCollection<TItem> ?? items.ToObservableCollection();
            _collection.CollectionChanged += OnCollectionChanged;
            _collection.ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            _valueProvider = valueProvider;
            _value = _valueProvider(_collection);
            _validators = new List<Func<TValue, string>>();
            Configure(configuration);
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

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            if (args.OldItems != null)
                args.OldItems.Cast<TItem>().ForEach(x => x.PropertyChanged -= OnItemPropertyChanged);
            if (args.NewItems != null)
                args.NewItems.Cast<TItem>().ForEach(x => x.PropertyChanged += OnItemPropertyChanged);
            Value = _valueProvider(_collection);
        }

        private void OnItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Value = _valueProvider(_collection);
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
    }
}