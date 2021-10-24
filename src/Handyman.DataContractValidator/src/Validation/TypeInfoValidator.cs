using Handyman.DataContractValidator.Model;
using System;

namespace Handyman.DataContractValidator.Validation
{
    internal abstract class TypeInfoValidator<T> : ITypeInfoValidator where T : TypeInfo
    {
        public void Validate(ITypeInfo actual, ITypeInfo expected, ValidationContext context)
        {
            if (!(expected is T e))
            {
                throw new ArgumentException();
            }

            if (!(actual is T a))
            {
                context.AddError($"type mismatch, expected '{e.TypeName}' but found '{actual.TypeName}'.");
                return;
            }

            Validate(a, e, context);
        }

        internal abstract void Validate(T actual, T expected, ValidationContext context);
    }
}