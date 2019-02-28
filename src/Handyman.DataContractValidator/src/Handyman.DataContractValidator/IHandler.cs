namespace Handyman.DataContractValidator
{
    internal interface IHandler
    {
        bool TryGetTypeName(object o, ValidationContext context, out string name);
        bool TryGetValidator(object dataContract, out IValidator validator);
    }
}