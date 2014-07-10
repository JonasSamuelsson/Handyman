using System;
using System.Collections.Generic;

namespace Handyman.Wpf
{
    public class ObservableValidationExpression<T>
    {
        public List<Func<T, string>> Validators { get; set; }
    }
}