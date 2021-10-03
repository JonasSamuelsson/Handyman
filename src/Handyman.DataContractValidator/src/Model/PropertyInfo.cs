namespace Handyman.DataContractValidator.Model
{
    internal class PropertyInfo
    {
        public bool IsIgnored { get; set; }
        public string Name { get; set; }
        public TypeInfo Type { get; set; }
    }
}