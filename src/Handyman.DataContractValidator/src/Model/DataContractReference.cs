using System;

namespace Handyman.DataContractValidator.Model
{
    internal class DataContractReference
    {
        public Func<object> Resolve { get; set; }
    }
}