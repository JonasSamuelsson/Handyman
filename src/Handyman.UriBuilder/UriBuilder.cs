using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman
{
    public class UriBuilder
    {
        private static readonly string NoValue = Guid.NewGuid().ToString();

        private Uri _baseAddress;
        private string _scheme;
        private string _host;
        private int? _port;
        private string _path;
        private string _query;
        private readonly List<KeyValuePair<string, string>> _queryParams = new List<KeyValuePair<string, string>>();
        private string _fragment;

        public static string DefaultScheme { get; set; }

        public UriBuilder Scheme(string scheme)
        {
            AssertNotNullOrWhitespace(scheme);
            _scheme = scheme;
            return this;
        }

        public UriBuilder Host(string host)
        {
            AssertNotNullOrWhitespace(host);
            _host = host;
            return this;
        }

        public UriBuilder Port(int? port)
        {
            AssertNotNull(port);
            _port = port;
            return this;
        }

        public UriBuilder Path(string path)
        {
            AssertNotNullOrWhitespace(path);
            _path = path.StartsWith("/") ? path : $"/{path}";
            return this;
        }

        public UriBuilder Query(string query)
        {
            _query = query ?? string.Empty;
            return this;
        }

        public UriBuilder QueryParams(string name)
        {
            return AddQueryParams(name, new[] { NoValue });
        }

        public UriBuilder QueryParams(string name, object value)
        {
            return value == null ? this : AddQueryParams(name, new[] { value });
        }

        public UriBuilder QueryParams(string name, string value)
        {
            return QueryParams(name, (object)value);
        }

        public UriBuilder QueryParams<T>(string name, IEnumerable<T> values)
        {
            return values == null ? this : AddQueryParams(name, values.OfType<object>());
        }

        private UriBuilder AddQueryParams(string name, IEnumerable<object> values)
        {
            foreach (var value in values)
            {
                var s = GetQueryParamValueAsString(value);
                _queryParams.Add(new KeyValuePair<string, string>(name, s));
            }
            return this;
        }

        private static string GetQueryParamValueAsString(object value)
        {
            if (value == null) return null;

            var valueType = value.GetType();

            if (valueType.GetTypeInfo().IsEnum)
            {
                var underlyingType = Enum.GetUnderlyingType(valueType);
                var underlyingTypeValue = Convert.ChangeType(value, underlyingType);
                return underlyingTypeValue.ToString();
            }

            return value.ToString();
        }

        public override string ToString()
        {
            var uri = string.Empty;

            if (_baseAddress != null || _host != null)
            {
                var scheme = _scheme ?? _baseAddress?.Scheme ?? DefaultScheme ?? "http";
                var host = _host ?? _baseAddress?.Host;
                uri = $"{scheme}://{host}";

                var port = _port ?? (_baseAddress?.IsDefaultPort == false ? _baseAddress.Port : -1);
                if (port != -1)
                {
                    uri += $":{port}";
                }
            }

            uri += _baseAddress?.AbsolutePath;
            if (_path != null)
            {
                uri += _path;
            }


            if (_query != null || _queryParams.Any())
            {
                uri += $"?{string.Join("&", new[] { _query, GetQueryParams(_queryParams) }.Where(s => !string.IsNullOrEmpty(s)))}";
            }

            if (_fragment != null)
            {
                uri += $"#{_fragment}";
            }

            return uri;
        }

        private static string GetQueryParams(IEnumerable<KeyValuePair<string, string>> @params)
        {
            return string.Join("&", @params.Select(GetQueryParam));
        }

        private static string GetQueryParam(KeyValuePair<string, string> kvp)
        {
            return kvp.Value == null
                ? kvp.Key
                : kvp.Value == NoValue
                    ? kvp.Key
                    : $"{kvp.Key}={System.Net.WebUtility.UrlEncode(kvp.Value)}";
        }

        public UriBuilder Fragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }

        private static void AssertNotNullOrWhitespace(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException();
        }

        private static void AssertNotNull(object value)
        {
            if (value == null) throw new ArgumentException();
        }

        public UriBuilder BaseAddress(string baseAddress)
        {
            _baseAddress = new Uri(baseAddress, UriKind.Absolute);
            return this;
        }
    }
}
