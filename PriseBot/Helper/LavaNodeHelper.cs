using Discord;
using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace PriseBot.Helper
{
    public static class LavaNodeHelper
    {
        private static readonly Random _random;

        static LavaNodeHelper ()
        {
            _random = new Random();
        }

        public static async Task ConnectAsync (SocketCommandContext context, LavaNode lavaNode, ILogger logger)
        {
            if (lavaNode.HasPlayer(context.Guild)) return;

            var channel = context.Guild.VoiceChannels.OrderByDescending(i => i.Users.Count).FirstOrDefault();

            if (channel == null) return;

            try
            {
                await lavaNode.JoinAsync(channel, context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                logger?.LogError($"Player couldn't join: {exception.Message}");
            }
        }

        public static async Task PlayAsync (SocketCommandContext context, LavaNode lavaNode, Database database)
        {

            var videoLength = await database.GetVideoLegth();
            var video = await database.GetVideoWithIndex(_random.Next(0, videoLength));


            var searchResponse = await lavaNode.SearchAsync(video);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
                return;

            var player = lavaNode.GetPlayer(context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                return;

            var track = searchResponse.Tracks[0];
            await player.PlayAsync(track);
        }

        public static async Task LeaveAsync (SocketCommandContext context, LavaNode lavaNode)
        {
            var player = lavaNode.GetPlayer(context.Guild);

            if (player == null) return;

            while (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                await Task.Delay(100);
            }

            await lavaNode.LeaveAsync(player.VoiceChannel);
        }

        public static async Task BuildAndSendEmbed (SocketCommandContext context, (string Header, string Value) args)
        {
            var embedBuilder = new EmbedBuilder()
                .WithThumbnailUrl("https://www.cigarworld.de/bilder/detail/small_500_265/2781_8717_41664.jpg")
                .WithDescription("Hier gibt's was zum priesen!")
                .WithColor(new Color(191, 89, 25))
                .AddField("Überschrift", args.Header, false)
                .AddField("Der Spruch", args.Value, false)
                .WithCurrentTimestamp();

            var embed = embedBuilder.Build();
            await context.Message.Channel.SendMessageAsync(null, false, embed);
        }

        public static async Task TTSAndDelete (SocketCommandContext context, (string Header, string Value) args)
        {
            var message = await context.Message.Channel.SendMessageAsync(args.Header, true);
            await Task.Delay(args.Header.Length * 100 + 2000);
            var message2 = await context.Message.Channel.SendMessageAsync(args.Value, true);

            _ = Task.Run(async () =>
              {
                  await Task.Delay(args.Value.Length * 100 + 2000);
                  await message.DeleteAsync();
                  await message2.DeleteAsync();
              });
        }
    }
}
