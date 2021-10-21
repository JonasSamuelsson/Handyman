using Handyman.DataContractValidator.Validation;
using System.Collections.Generic;

namespace Handyman.DataContractValidator.Model
{
    internal class EnumTypeInfo : TypeInfo
    {
        public bool IsFlags { get; set; }
        public Dictionary<long, string> Values { get; set; }

        public override string TypeName => "enum";

        public override ITypeInfoValidator GetValidator()
        {
            return new EnumValidator();
        }

        {
        }
    }
}