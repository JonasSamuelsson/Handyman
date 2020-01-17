namespace Handyman.Tools.Outdated.IO
{
    public interface IProcessRunner
    {
        IProcessInfo Start(ProcessStartInfo startInfo);
    }
}