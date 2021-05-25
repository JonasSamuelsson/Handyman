using Handyman.AspNetCore.ETags.Internals.ModelBinding;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Net.Http.Headers;
using System;
using System.Linq;

namespace Handyman.AspNetCore.ETags.Internals.AppModel
{
    internal class ETagParameterConvention : IParameterModelConvention
    {
        public void Apply(ParameterModel parameter)
        {
            if (!IsETagParameter(parameter, out var headerName, out var isRequired))
            {
                return;
            }

            parameter.Action.Filters.Add(new ETagMetadata
            {
                HeaderName = headerName,
                IsRequired = isRequired
            });

            parameter.BindingInfo = new BindingInfo
            {
                BinderModelName = headerName,
                BindingSource = BindingSource.Header,
                BinderType = typeof(ETagModelBinder)
            };
        }

        private static bool IsETagParameter(ParameterModel parameter, out string headerName, out bool isRequired)
        {
            if (IsIfMatchParameter(parameter))
            {
                headerName = HeaderNames.IfMatch;
                isRequired = true;
                return true;
            }

            if (IsIfNoneMatchParameter(parameter))
            {
                headerName = HeaderNames.IfNoneMatch;
                isRequired = false;
                return true;
            }

            headerName = default;
            isRequired = default;
            return false;
        }

        private static bool IsIfMatchParameter(ParameterModel parameter)
        {
            if (parameter.Attributes.OfType<FromIfMatchHeaderAttribute>().Any())
                return true;

            if (NameEquals(parameter, "ifMatch"))
                return true;

            if (NameEquals(parameter, "ifMatchETag"))
                return true;

            return false;
        }

        private static bool IsIfNoneMatchParameter(ParameterModel parameter)
        {
            if (parameter.Attributes.OfType<FromIfNoneMatchHeaderAttribute>().Any())
                return true;

            if (NameEquals(parameter, "ifNoneMatch"))
                return true;

            if (NameEquals(parameter, "ifNoneMatchETag"))
                return true;

            return false;
        }

        private static bool NameEquals(ParameterModel parameter, string name)
        {
            return parameter.Name.Equals(name, StringComparison.OrdinalIgnoreCase);
        }
    }
}