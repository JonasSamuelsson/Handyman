namespace Handyman.AspNetCore.ApiVersioning.Abstractions
{
    internal class ApiVersionDescriptor
    {
        public IApiVersion[] Versions { get; set; }
        public bool IsOptional { get; set; }
        public string DefaultVersion { get; set; }
        public string ErrorMessage { get; set; }
    }
}