using System.Collections.Generic;

namespace Handyman.AspNetCore.ApiVersioning.Abstractions;

internal interface IApiVersionDescriptorProvider
{
    ApiVersionDescriptor GetApiVersionDescriptor(string endpointDisplayName, IReadOnlyCollection<string> versions, string nullableDefaultVersion, bool? isOptional);
}