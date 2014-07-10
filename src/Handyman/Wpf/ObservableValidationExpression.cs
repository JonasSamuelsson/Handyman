using System;
using System.Collections.Generic;

namespace Handyman.Wpf
{
    public class ObservableValidationExpression<T>
    {
        public IList<Func<T, string>> Validators { get; internal set; }
    }
}