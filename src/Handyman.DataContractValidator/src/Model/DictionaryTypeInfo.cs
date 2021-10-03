namespace Handyman.DataContractValidator.Model
{
    internal class DictionaryTypeInfo : TypeInfo
    {
        public TypeInfo Key { get; set; }
        public TypeInfo Value { get; set; }
        public override string TypeName => "dictionary";
    }
}