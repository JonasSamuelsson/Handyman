namespace Handyman.Tools.Docs.Utils
{
    public interface IValueConverter
    {
        bool TryConvertFromString(string s, ILogger logger, out object value);
        string ConvertToString(object value);
    }
}