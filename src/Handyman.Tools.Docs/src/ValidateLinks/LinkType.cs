using System;

namespace Handyman.Tools.Docs.ValidateLinks;

[Flags]
public enum LinkType
{
    Local = 1,
    Remote = 2,
    All = Local | Remote
}