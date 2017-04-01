using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Handyman
{
    public class UriBuilder
    {
        private string _scheme = string.Empty;
        private string _host = string.Empty;
        private int? _port;
        private string _path = string.Empty;
        private string _query = string.Empty;
        private readonly List<KeyValuePair<string, string>> _queryParams = new List<KeyValuePair<string, string>>();
        private string _fragment = string.Empty;
        private bool _isBasedOnUri;

        public static string DefaultScheme { get; set; }

        public UriBuilder BaseAddress(string baseAddress)
        {
            Uri uri;
            Uri.TryCreate(baseAddress, UriKind.RelativeOrAbsolute, out uri);
            _isBasedOnUri = uri.IsAbsoluteUri;

            if (_isBasedOnUri)
            {
                Scheme(uri.Scheme);
                Host(uri.Host);
                Port(uri.IsDefaultPort ? default(int?) : uri.Port);
                _path = uri.AbsolutePath != "/" ? uri.AbsolutePath : string.Empty;
            }
            else
                Path(uri);
            return this;
        }

        public UriBuilder Scheme(string scheme)
        {
            _scheme = scheme ?? string.Empty;
            _isBasedOnUri = true;
            return this;
        }

        public UriBuilder Host(string host)
        {
            _host = host ?? string.Empty;
            _isBasedOnUri = true;
            return this;
        }

        public UriBuilder Port(int? port)
        {
            _port = port;
            _isBasedOnUri = true;
            return this;
        }

        public UriBuilder Path(params object[] segments)
        {
            var path = string.Join("/", segments.Select(x => x.ToString().Trim('/')));
            _path = $"{(_isBasedOnUri && !_path.StartsWith("/") ? "/" : string.Empty)}{_path}" == string.Empty
               ? path
               : $"{_path}/{path}";
            return this;
        }

        public UriBuilder Query(string query)
        {
            _query = query ?? string.Empty;
            return this;
        }

        public UriBuilder QueryParams(string name)
        {
            return AddQueryParams(name, new[] { string.Empty });
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

            if (_host != string.Empty)
            {
                var scheme = _scheme == string.Empty ? (DefaultScheme ?? "http") : _scheme;
                uri = $"{scheme}://{_host}";

                if (_port.HasValue)
                {
                    uri += $":{_port.Value}";
                }
            }

            if (_path != string.Empty)
            {
                uri += _path;
            }

            if (_query != string.Empty || _queryParams.Any())
            {
                uri += $"?{string.Join("&", new[] { _query, GetQueryParams(_queryParams) }.Where(s => !string.IsNullOrEmpty(s)))}";
            }

            if (_fragment != string.Empty)
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
               : $"{kvp.Key}={System.Net.WebUtility.UrlEncode(kvp.Value)}";
        }

        public UriBuilder Fragment(string fragment)
        {
            _fragment = fragment;
            return this;
        }
    }
}
