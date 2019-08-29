namespace Handyman.Azure.Functions.Http.ApiVersioning
{
    internal static class HeaderNames
    {
        public static string DefaultVersion => "x-handyman-azure-functions-http-apiVersioning-defaultVersion";
        public static string Optional => "x-handyman-azure-functions-http-apiVersioning-optional";
        public static string ValidVersions => "x-handyman-azure-functions-http-apiVersioning-validVersions";
    }
}