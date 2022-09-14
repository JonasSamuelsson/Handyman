namespace Handyman.DataContractValidator
{
    public class DataContractGeneratorOptions
    {
        public string Indentation { get; set; } = "   ";
        public bool SortPropertiesAlphabetically { get; set; } = false;

        internal DataContractGeneratorOptions Clone()
        {
            return new DataContractGeneratorOptions
            {
                Indentation = Indentation,
                SortPropertiesAlphabetically = SortPropertiesAlphabetically
            };
        }
    }
}