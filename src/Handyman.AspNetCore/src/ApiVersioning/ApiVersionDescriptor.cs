namespace Handyman.AspNetCore.ApiVersioning
{
    internal class ApiVersionDescriptor
    {
        public IApiVersion[] Versions { get; set; }
        public bool Optional { get; set; }
        public string DefaultVersion { get; set; }
        public string ErrorMessage { get; set; }
    }
}