using Handyman.AspNetCore.ApiVersioning.Schemes.Literal;
using System;

namespace Handyman.AspNetCore.ApiVersioning
{
    public class ApiVersioningOptions
    {
        internal Type ApiVersionParserType { get; set; }

        public void UseLiteralScheme()
        {
            ApiVersionParserType = typeof(LiteralApiVersionParser);
        }
    }
}