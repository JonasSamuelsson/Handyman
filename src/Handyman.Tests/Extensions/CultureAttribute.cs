using System;
using System.Globalization;

namespace Handyman.Tests.Extensions
{
    public class CultureAttribute : Attribute
    {
        private readonly string _name;

        public CultureAttribute(string name)
        {
            _name = name;
        }

        public CultureInfo GetCulture()
        {
            return CultureInfo.GetCultureInfo(_name);
        }
    }
}