using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Handyman.Mediator.RequestPipelineCustomization;

namespace Handyman.Mediator
{
    public class ServiceValidator
    {
        public void Validate(IEnumerable<Type> serviceTypes)
        {
            var requests = serviceTypes
                .Where(x => x.IsGenericType)
                .Where(x => x.GetGenericTypeDefinition() == typeof(IRequestHandler<,>))
                .Select(x => x.GetGenericArguments().First())
                .GroupBy(x => x)
                .Select(x => new { type = x.Key, handlers = x.Count() })
                .Where(x => x.handlers != 1)
                .OrderBy(x => x.type.FullName);

            var errors = new List<string>();

            foreach (var request in requests)
            {
                if (request.type.GetCustomAttributes().OfType<RequestPipelineBuilderAttribute>().Any())
                    continue;

                errors.Add($"Request of type '{request.type.FullName}' has multiple handlers registered but does not use a customized pipeline, this would result in a runtime error.");
            }

            if (!errors.Any())
                return;

            var builder = new StringBuilder();
            builder.AppendLine("Invalid service configuration.");
            errors.ForEach(x => builder.AppendLine(x));

            throw new InvalidOperationException(builder.ToString().TrimEnd());
        }
    }
}