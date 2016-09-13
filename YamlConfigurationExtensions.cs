using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;

namespace YamlConfiguration
{
    public static class YamlConfigurationExtensions
    {
        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: false, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: false);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
        {
            return AddYamlFile(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
        }

        public static IConfigurationBuilder AddYamlFile(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }
            
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("InvalidFilePath", nameof(path));
            }

            var source = new YamlConfigurationSource
            {
                FileProvider = provider,
                Path = path,
                Optional = optional,
                ReloadOnChange = reloadOnChange
            };

            builder.Add(source);
            return builder;
        }
    }
}