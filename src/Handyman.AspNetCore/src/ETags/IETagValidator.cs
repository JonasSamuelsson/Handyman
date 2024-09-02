namespace Handyman.AspNetCore.ETags;

internal interface IETagValidator
{
    bool IsValidETag(string candidate);
}