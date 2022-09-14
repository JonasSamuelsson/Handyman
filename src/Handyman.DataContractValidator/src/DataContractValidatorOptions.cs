using System;

namespace Handyman.DataContractValidator
{
    public class DataContractValidatorOptions
    {
        public bool AllowPropertiesNotFoundInDataContract { get; set; } = false;
        public StringComparison EnumValueNameComparison { get; set; } = StringComparison.Ordinal;
        public StringComparison PropertyNameComparison { get; set; } = StringComparison.Ordinal;

        internal DataContractValidatorOptions Clone()
        {
            return new DataContractValidatorOptions
            {
                AllowPropertiesNotFoundInDataContract = AllowPropertiesNotFoundInDataContract,
                EnumValueNameComparison = EnumValueNameComparison,
                PropertyNameComparison = PropertyNameComparison
            };
        }
    }
}