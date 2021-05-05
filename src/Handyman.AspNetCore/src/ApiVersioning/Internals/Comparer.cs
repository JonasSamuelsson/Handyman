using Microsoft.AspNetCore.Mvc.ApiExplorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Handyman.AspNetCore.ApiVersioning.Internals
{
    internal class Comparer : IComparer<ApiDescription>, IComparer<ApiVersionMetadata>, IComparer<IApiVersion>
    {
        internal static readonly Comparer Instance = new Comparer();

        public int Compare(ApiDescription x, ApiDescription y)
        {
            var xMetadata = x.ActionDescriptor.EndpointMetadata.OfType<ApiVersionMetadata>().SingleOrDefault();
            var yMetadata = y.ActionDescriptor.EndpointMetadata.OfType<ApiVersionMetadata>().SingleOrDefault();

            return Compare(xMetadata, yMetadata);
        }

        public int Compare(ApiVersionMetadata x, ApiVersionMetadata y)
        {
            var xVersion = x?.Versions.First();
            var yVersion = y?.Versions.First();

            return Compare(xVersion, yVersion);
        }

        public int Compare(IApiVersion x, IApiVersion y)
        {
            if (x != null && y != null)
            {
                if (x.GetType() == y.GetType() && x is IComparable comparable)
                    return comparable.CompareTo(y);

                return 0;
            }

            if (x == null && y == null)
                return 0;

            return x == null ? -1 : 1;
        }
    }
}