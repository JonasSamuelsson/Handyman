using System;

namespace Handyman.Tools.Docs.Utils
{
    public class ValueConverterAttribute : Attribute
    {
        public ValueConverterAttribute(Type valueConverterType)
        {
            ValueConverterType = valueConverterType;
        }

        public Type ValueConverterType { get; }
    }
}