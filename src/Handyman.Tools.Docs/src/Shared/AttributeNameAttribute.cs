using System;

namespace Handyman.Tools.Docs.Shared;

public class AttributeNameAttribute : Attribute
{
    public string Name { get; }

    public AttributeNameAttribute(string name)
    {
        Name = name;
    }
}