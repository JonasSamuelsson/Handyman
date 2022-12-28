namespace Handyman.Tools.Docs.Shared;

public interface IAttributesConverter
{
    public TAttributes ConvertTo<TAttributes>(Attributes attributes) where TAttributes : new();
}