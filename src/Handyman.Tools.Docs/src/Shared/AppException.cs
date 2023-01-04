using System;

namespace Handyman.Tools.Docs.Shared;

public class AppException : Exception
{
    public AppException(string message) : base(message)
    {
    }
}