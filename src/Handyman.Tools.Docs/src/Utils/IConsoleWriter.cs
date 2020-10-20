namespace Handyman.Tools.Docs.Utils
{
    public interface IConsoleWriter
    {
        void WriteError(string message);
        void WriteInfo(string message);
    }
}