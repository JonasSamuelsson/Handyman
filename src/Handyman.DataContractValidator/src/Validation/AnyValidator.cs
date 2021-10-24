using Handyman.DataContractValidator.Model;
using System.Diagnostics;

namespace Handyman.DataContractValidator.Validation
{
    internal class AnyValidator : ITypeInfoValidator
    {
        public void Validate(ITypeInfo actual, ITypeInfo expected, ValidationContext context)
        {
            Debug.Assert(expected is AnyTypeInfo);
        }
    }
}