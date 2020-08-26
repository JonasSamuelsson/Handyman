using System;

namespace Handyman.DuckTyping
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PropertyAttribute : Attribute
    {
        public PropertyAttribute(Type type, string name)
        {
            Type = type;
            Name = name;
        }

        public Type Type { get; }
        public string Name { get; }

        public bool Ignore { get; set; }
    }
}