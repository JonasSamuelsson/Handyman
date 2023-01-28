using System;

namespace Handyman.Tools.Docs.Shared;

public class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }

    public AppException(Exception exception) : base(exception.Message, exception)
    {
        IsHandled = true;
    }

    public bool IsHandled { get; set; }
}