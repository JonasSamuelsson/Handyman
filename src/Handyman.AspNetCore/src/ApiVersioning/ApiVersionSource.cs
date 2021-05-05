using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Handyman.AspNetCore.ApiVersioning
{
    public class ApiVersionSource
    {
        public BindingSource BindingSource { get; set; }
        public string Name { get; set; }
    }
}