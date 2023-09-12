using System.Reflection;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WebApi.Configuration {

    public class WebFileStorageConfigSource : IConfigurationSource {

        public IConfigurationProvider Build(IConfigurationBuilder builder) {
            return new WebFileStorageConfigProvider();
        }

    }

    public class WebFileStorageConfigProvider : ConfigurationProvider {

        public override void Load() {

            string configText;
            try {
                configText = File.ReadAllText("config.yml");
            } catch (Exception ex) {
                throw new Exception("""
                    Can not load "config.yml"
                    """, ex);
            }

            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var config = deserializer.Deserialize<AppConfig>(configText);

            Data = config.ToDictionary();

        }

    }

    public static class WebFileStorageConfigExtension {

        public static IConfigurationBuilder AddWFSConfiguration(this IConfigurationBuilder builder) {
            return builder.Add(new WebFileStorageConfigSource());
        }

        public static IConfigurationSection GetWFSConfig(this IConfiguration configuration) {
            return configuration.GetSection("");
        }

    }

    public class DictableConfig {

        private readonly Dictionary<string, string?> _configDict = new();

        public Dictionary<string, string?> ToDictionary() {

            foreach (var prop in GetType().GetProperties()) {
                var value = prop.GetValue(this);
                ValueSetter(prop.Name, value);
            }

            return _configDict;
        }

        private void ValueSetter(string name, object? value) {
            if (value is null) {
                return;
            }

            if (value is DictableConfig dcInst) {
                foreach (var item in dcInst.ToDictionary()) {
                    _configDict.Add($"{name}:{item.Key}", item.Value);
                }
            } else if (value is System.Collections.IList listInst) {
                for (int i = 0; i < listInst.Count; i++) {
                    ValueSetter($"{name}:{i}", listInst[i]);
                }
            } else {
                _configDict.Add(name, value?.ToString() ?? "");
            }
        }

    }

    public class AppConfig : DictableConfig {

        public AppConfig() { }

        public AppConfig(IConfiguration configuration) {
            configuration.Bind(this);
        }

        public StorageConfig Storage { get; set; } = new();
        public ServerConfig Server { get; set; } = new();
        public SecurityConfig Security { get; set; } = new();
        public WebUiConfig WebUi { get; set; } = new();
    }

    public class StorageConfig : DictableConfig {
        public string Root { get; set; } = ".";
    }

    public class ServerConfig : DictableConfig {
        public List<string> Hosts { get; set; } = new();
        public HttpConfig Http { get; set; } = new();
        public HttpsConfig Https { get; set; } = new();
    }

    public class HttpConfig : DictableConfig {
        public bool Enable { get; set; } = true;
        public string IpAddress { get; set; } = "0.0.0.0";
        public int Port { get; set; } = 80;
    }

    public class HttpsConfig : DictableConfig {
        public bool Enable { get; set; } = false;
        public string IpAddress { get; set; } = "0.0.0.0";
        public int Port { get; set; } = 443;
        public string SslPfx { get; set; } = "";
        public string SslPwd { get; set; } = "";
    }

    public class SecurityConfig : DictableConfig {
        public bool Enable { get; set; }
        public string UsersFile { get; set; } = "";
    }

    public class WebUiConfig : DictableConfig {
        public string BackgroundImage { get; set; } = "";
    }

}
