namespace Handyman.AspNetCore.ApiVersioning.Internals
{
    internal class ApiVersionMetadata
    {
        public IApiVersion[] Versions { get; set; }
        public bool IsOptional { get; set; }
        public IApiVersion DefaultVersion { get; set; }
    }
}