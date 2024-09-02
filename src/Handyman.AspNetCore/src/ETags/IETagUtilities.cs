namespace Handyman.AspNetCore.ETags;

public interface IETagUtilities
{
    IETagComparer Comparer { get; }
    IETagConverter Converter { get; }
}