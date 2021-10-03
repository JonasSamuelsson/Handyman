using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal interface ITypeInfoValidator
    {
        bool TryValidate(TypeInfo actual, TypeInfo expected, ValidationContext context);
    }

    //internal static class TypeInfoValidatorExtensions
    //{
    //    internal static void Validate<T>(this TypeInfoValidator<T> validator, T actual, T expected) where T : TypeInfo
    //    {
    //        var errors = new List<string>();

    //        validator.Validate(actual, expected, errors);

    //        if (errors.Any())
    //        {
    //            var message = string.Join(Environment.NewLine, errors);
    //            throw new InvalidOperationException(message);
    //        }
    //    }
    //}
}