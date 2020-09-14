using System;

namespace Handyman.DuckTyping
{
    [AttributeUsage(AttributeTargets.Interface, AllowMultiple = false)]
    public class DuckTypedObjectContractAttribute : Attribute
    {
    }
}