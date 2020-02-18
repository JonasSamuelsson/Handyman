using System;
using Handyman.AspNetCore.ApiVersioning.Schemes.Literal;

namespace Handyman.AspNetCore.ApiVersioning
{
    public class ApiVersioningOptions
    {
        internal Type ApiVersionParserType { get; set; }

        public void UseLiteralApiVersioningScheme()
        {
            ApiVersionParserType = typeof(LiteralApiVersionParser);
        }
    }
}