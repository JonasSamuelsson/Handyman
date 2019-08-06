using Microsoft.Azure.Cosmos.Table;
using System;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Handyman.Azure.Cosmos.Table
{
    internal class CloudTableFactory
    {
        private static readonly Strategy[] Strategies = {
            new Strategy
            {
                ParameterTypes = new[] {typeof(StorageUri), typeof(StorageCredentials)},
                Factory = CreateTableFromStorageUriAndStorageCredentials
            },
            new Strategy
            {
                ParameterTypes = new[] {typeof(Uri), typeof(StorageCredentials)},
                Factory = CreateTableFromUriAndStorageCredentials
            },
            new Strategy
            {
                ParameterTypes = new[] {typeof(Uri)},
                Factory = CreateTableFromUri
            }
        };

        internal static CloudTable CreateCloudTable(Type cloudTableType, CloudTableClient client, string tableName)
        {
            var constructors = cloudTableType.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            foreach (var strategy in Strategies)
            {
                foreach (var constructor in constructors)
                {
                    var parameterTypes = constructor.GetParameters().Select(x => x.ParameterType).ToArray();

                    if (strategy.ParameterTypes.Length != parameterTypes.Length)
                        continue;

                    var match = true;

                    for (var i = 0; i < strategy.ParameterTypes.Length; i++)
                    {
                        match &= strategy.ParameterTypes[i] == parameterTypes[i];
                    }

                    if (!match) continue;

                    return strategy.Factory.Invoke(cloudTableType, client, tableName);
                }
            }

            var builder = new StringBuilder();
            builder.AppendLine($"Type '{cloudTableType.FullName}' must have a public constructor taking any of the following combinations of parameters;");

            foreach (var strategy in Strategies)
            {
                builder.AppendLine(string.Join(", ", strategy.ParameterTypes.Select(x => x.Name)));
            }

            throw new InvalidOperationException(builder.ToString().Trim());
        }

        private static CloudTable CreateTableFromUri(Type type, CloudTableClient client, string tableName)
        {
            var uri = AppendPathToUri(client.StorageUri.PrimaryUri, tableName);
            var args = new object[] { uri };

            return (CloudTable)Activator.CreateInstance(type, args);
        }

        private static CloudTable CreateTableFromUriAndStorageCredentials(Type type, CloudTableClient client, string tableName)
        {
            var uri = AppendPathToUri(client.StorageUri.PrimaryUri, tableName);
            var storageCredentials = client.Credentials;
            var args = new object[] { uri, storageCredentials };

            return (CloudTable)Activator.CreateInstance(type, args);
        }

        private static CloudTable CreateTableFromStorageUriAndStorageCredentials(Type type, CloudTableClient client, string tableName)
        {
            var storageUri = AppendPathToUri(client.StorageUri, tableName);
            var storageCredentials = client.Credentials;
            var args = new object[] { storageUri, storageCredentials };

            return (CloudTable)Activator.CreateInstance(type, args);
        }

        private static StorageUri AppendPathToUri(StorageUri storageUri, string relativeUri)
        {
            var primaryUri = AppendPathToUri(storageUri.PrimaryUri, relativeUri);
            var secondaryUri = AppendPathToUri(storageUri.SecondaryUri, relativeUri);

            return new StorageUri(primaryUri, secondaryUri);
        }

        private static Uri AppendPathToUri(Uri uri, string relativeUri)
        {
            if (uri == null || relativeUri.Length == 0)
                return uri;

            var separator = Uri.EscapeUriString("/");
            relativeUri = Uri.EscapeUriString(relativeUri);
            var uriBuilder = new UriBuilder(uri);
            uriBuilder.Path += !uriBuilder.Path.EndsWith(separator, StringComparison.Ordinal) ? separator + relativeUri : relativeUri;
            return uriBuilder.Uri;
        }

        private class Strategy
        {
            public Type[] ParameterTypes { get; set; }
            public Func<Type, CloudTableClient, string, CloudTable> Factory { get; set; }
        }
    }
}