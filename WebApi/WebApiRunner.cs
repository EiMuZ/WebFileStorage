﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Diagnostics;
using System.Net;
using WebApi.Configuration;
using WebApi.Security;
using WebApi.Services;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace WebApi {

    public class WebApiRunner {

        public static Action<WebApplication> BeforeRun { get; set; } = PrepareApplication;

        private static AppConfig? Config { get; set; }

        public static void Run(string[] args) {
            var app = Build(args);
            BeforeRun(app);
            app.Run();
        }

        private static WebApplication Build(string[] args) {

            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration.AddWFSConfiguration();

            Config = new AppConfig();
            builder.Configuration.Bind(Config);
            builder.Services.AddSingleton<AppConfig>();

            builder.WebHost.UseKestrel(options => ConfigureServer(Config.Server, options));
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddControllers();
            builder.Services.AddSingleton<List<User>>();
            builder.Services.AddScoped<FileService>();
            builder.Services.AddSingleton<IdentityService>();
            builder.Services.AddSingleton<JwtHandler>();
            builder.Services.AddScoped<WebUiSupportService>();
            builder.Services.AddSingleton<SystemInfoService>();

            if (Config.Security.Enable) {
                builder.Services.AddAuthentication(ConfigureAuthenticationHandler<JwtAuthenticationHandler>);
            } else {
                builder.Services.AddAuthentication(ConfigureAuthenticationHandler<PermitAllAuthenticationHandler>);
            }

            return builder.Build();

        }

        private static void ConfigureAuthenticationHandler<T>(AuthenticationOptions options) where T : IAuthenticationHandler {
            options.AddScheme<T>(typeof(T).Name, typeof(T).Name);
            options.DefaultAuthenticateScheme = typeof(T).Name;
            options.DefaultChallengeScheme = typeof(T).Name;
            options.DefaultForbidScheme = typeof(T).Name;
        }

        private static void ConfigureServer(ServerConfig config, KestrelServerOptions options) {
            var http = config.Http;
            if (http.Enable) {
                options.Listen(IPAddress.Parse(http.IpAddress), http.Port);
            }
            var https = config.Https;
            if (https.Enable) {
                var sslPwdContent = File.ReadAllText(https.SslPwd);
                options.Listen(IPAddress.Parse(https.IpAddress), https.Port, configure => {
                    configure.UseHttps(https.SslPfx, sslPwdContent);
                });
            }
            options.Limits.KeepAliveTimeout = new TimeSpan(24, 0, 0);
            options.Limits.MaxConcurrentConnections = 100;
            options.Limits.MaxConcurrentUpgradedConnections = 100;
            options.Limits.MaxRequestBodySize = long.MaxValue;
        }

        private static void PrepareApplication(WebApplication app) {

            Debug.Assert(Config is not null);
            Directory.CreateDirectory(Config.Storage.Root);

            app.UseAuthentication();
            app.UseAuthorization();
            if (Config.Security.Enable) {

                var users = app.Services.GetRequiredService<List<User>>();
                Debug.Assert(users is not null);
                users.AddRange(ReadUsersFromFile(Config.Security.UsersFile));
            }

            app.UseCors(policyBuilder => {
                policyBuilder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });

            app.MapControllers();
        }

        private static IEnumerable<User> ReadUsersFromFile(string usersFile) {
            var yamlDeserializer = new DeserializerBuilder()
                .WithNamingConvention(HyphenatedNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            var users = yamlDeserializer.Deserialize<List<User>>(File.ReadAllText(usersFile));

            return users.DistinctBy(u => u.Username)
                .Where(u => !string.IsNullOrWhiteSpace(u.Username) && !string.IsNullOrWhiteSpace(u.Password));
        }

    }

}
