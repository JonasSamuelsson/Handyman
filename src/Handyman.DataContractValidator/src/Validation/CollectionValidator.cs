using Handyman.DataContractValidator.Model;

namespace Handyman.DataContractValidator.Validation
{
    internal class CollectionValidator : TypeInfoValidator<CollectionTypeInfo>
    {
        internal override void Validate(CollectionTypeInfo actual, CollectionTypeInfo expected, ValidationContext context)
        {
            context.Validate("Item", actual.Item, expected.Item);
        }
    }
}