namespace Handyman.AspNetCore.ApiVersioning.Abstractions
{
    internal class ApiVersionDescriptor
    {
        public IApiVersion[] Versions { get; set; }
        public bool IsOptional { get; set; }
        public IApiVersion DefaultVersion { get; set; }
    }
}