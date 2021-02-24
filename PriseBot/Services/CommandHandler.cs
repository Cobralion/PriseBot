using Discord;
using Discord.Addons.Hosting;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Infrastructure.Enities;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Services
{
    public class CommandHandler : InitializedService
    {
        private readonly IServiceProvider _provider;
        private readonly DiscordSocketClient _client;
        private readonly CommandService _service;
        private readonly IConfiguration _config;
        private readonly Database _database;
        private readonly LavaNode _lavaNode;

        public CommandHandler(IServiceProvider provider, DiscordSocketClient client, CommandService service, IConfiguration config, Database database, LavaNode lavaNode)
        {
            _database = database;
            _provider = provider;
            _client = client;
            _service = service;
            _config = config;
            _lavaNode = lavaNode;
        }

        public override async Task InitializeAsync(CancellationToken cancellationToken)
        {
            _client.Ready += OnReadyAsync;
            _client.MessageReceived += OnMessageReceived;
            _service.CommandExecuted += OnCommandExecuted;
            await _service.AddModulesAsync(Assembly.GetEntryAssembly(), _provider);
        }

        private async Task OnReadyAsync()
        {
            if (!_lavaNode.IsConnected)
                await _lavaNode.ConnectAsync();
        }

        private async Task OnMessageReceived(SocketMessage arg)
        {
            if (!(arg is SocketUserMessage message)) return;
            if (message.Source != MessageSource.User) return;

            var argPos = 0;
            if (!message.HasStringPrefix(_config["prefix"], ref argPos) && !message.HasMentionPrefix(_client.CurrentUser, ref argPos)) return;

            var context = new SocketCommandContext(_client, message);

            var guildSettings = await _database.GetGuildSettings(context.Guild.Id.ToString());

            if (guildSettings == null)
            {
                await _database.SetGuildSettings(context.Guild.Id.ToString(), new GuildSettings(context.Channel.Id.ToString(), _config["minimum-time"], _config["maximum-time"]));
                await context.Channel.SendMessageAsync($"Channel has been registered as main bot channel.");
                return;
            }

            if (guildSettings.Channel != message.Channel.Id.ToString()) return;

            await _service.ExecuteAsync(context, argPos, _provider);
        }

        private async Task OnCommandExecuted(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (command.IsSpecified && !result.IsSuccess) await context.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}
