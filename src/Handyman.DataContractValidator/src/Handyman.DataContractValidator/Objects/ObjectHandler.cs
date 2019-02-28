namespace Handyman.DataContractValidator.Objects
{
    internal class ObjectHandler : IHandler
    {
        public bool TryGetTypeName(object o, ValidationContext context, out string name)
        {
            name = "complex-type";
            return true;
        }

        public bool TryGetValidator(object dataContract, out IValidator validator)
        {
            validator = new ObjectValidator(dataContract);
            return true;
        }
    }
}