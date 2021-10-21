using Handyman.DataContractValidator.Validation;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class ObjectTypeInfo : TypeInfo
    {
        public IEnumerable<PropertyInfo> Properties { get; set; }
        public override string TypeName => "object";

        public override ITypeInfoValidator GetValidator()
        {
            return new ObjectValidator();
        }
    }
}