using Handyman.DataContractValidator.Model;
using System;
using System.Linq;

namespace Handyman.DataContractValidator.Validation
{
    internal abstract class TypeInfoValidator<T> : ITypeInfoValidator where T : ITypeInfo
    {
        public void Validate(ITypeInfo actual, ITypeInfo expected, DataContractValidatorContext context)
        {
            if (!(expected is T e))
            {
                throw new ArgumentException();
            }

            if (!(actual is T a))
            {
                context.AddError($"type mismatch, expected '{e.Name}' but found '{actual.Name}'.");
                return;
            }

            if (new[] { a, e }.Count(x => x.IsNullable == true) == 1)
            {
                context.AddError($"type mismatch, expected '{e.Name}' but found '{actual.Name}'.");
                return;
            }

            Validate(a, e, context);
        }

        internal abstract void Validate(T actual, T expected, DataContractValidatorContext context);
    }
}