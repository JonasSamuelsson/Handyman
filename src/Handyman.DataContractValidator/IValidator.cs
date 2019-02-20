using System;

namespace Handyman.DataContractValidator
{
    internal interface IValidator
    {
        string GetTypeName(ValidationContext context);
        void Validate(Type type, ValidationContext context);
    }
}