using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PriseBot.Helper;
using PriseBot.Worker;
using System;
using System.IO;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", "./firebase-key.json");
            Environment.SetEnvironmentVariable("DISCORD_API_TOKEN", await File.ReadAllTextAsync("./token.txt"));

            var builder = new HostBuilder()
                .ConfigureAppConfiguration(x =>
                {
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", false, true)
                        .Build();

                    x.AddConfiguration(configuration);
                    x.AddEnvironmentVariables();
                })
                .ConfigureLogging(x =>
                {
                    x.AddConsole();
                    x.SetMinimumLevel(LogLevel.Debug); // Defines what kind of information should be logged (e.g. Debug, Information, Warning, Critical) adjust this to your liking
                })
                .ConfigureDiscordHost<DiscordSocketClient>((context, config) =>
                {
                    config.SocketConfig = new DiscordSocketConfig
                    {
                        LogLevel = LogSeverity.Verbose, // Defines what kind of information should be logged from the API (e.g. Verbose, Info, Warning, Critical) adjust this to your liking
                        AlwaysDownloadUsers = true,
                        MessageCacheSize = 200,
                    };

                    config.Token = context.Configuration["DISCORD_API_TOKEN"];
                })
                .UseCommandService((context, config) =>
                {
                    config.CaseSensitiveCommands = false;
                    config.LogLevel = LogSeverity.Verbose;
                    config.DefaultRunMode = RunMode.Async; //Async or Sync
                })
                .ConfigureServices((context, services) =>
                {
                    services.AddHostedService<Services.CommandHandler>();
                    services.AddSingleton<Database>();
                    services.AddSingleton<WorkerStore>();
                    services.AddLavaNode(x =>
                    {
                        x.SelfDeaf = true;
                        x.LogSeverity = LogSeverity.Verbose;
                    });
                })
                .UseConsoleLifetime();

            var host = builder.Build();
            using (host)
            {
                try
                {
                    await host.RunAsync();
                }
                catch {}
            }
        }
    }
}
