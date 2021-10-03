using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal abstract class TypeInfoValidator<T> : ITypeInfoValidator where T : TypeInfo
    {
        public bool TryValidate(TypeInfo actual, TypeInfo expected, ValidationContext context)
        {
            if (expected is T)
            {
                Validate((T)actual, (T)expected, context);
                return true;
            }

            return false;
        }

        internal abstract void Validate(T actual, T expected, ValidationContext context);
    }
}