using Discord;
using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using PriseBot.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Victoria;
using Victoria.Enums;

namespace PriseBot.Worker
{
    public sealed class ChargeWorker : IWorker
    {
        private readonly object _lockObject = new object();
        private bool _running = false;
        private Thread _thread;
        private readonly Random _random;

        private SocketCommandContext _context;
        private MinMaxTime _time;
        private LavaNode _lavaNode;
        private Database _database;

        public string Guild { get; private set; }

        public ChargeWorker()
        {
            _random = new Random();
        }

        public bool IsRunning()
        {
            return _running;
        }

        public void Configure(WorkerInformation workerInformation)
        {
            Guild = workerInformation.Guild;
            _context = workerInformation.Context;
            _time = workerInformation.Time;
            _lavaNode = workerInformation.LavaNode;
            _database = workerInformation.Database;
            _thread = new Thread(Charge);
        }

        public void Start()
        {
            lock (_lockObject)
            {
                _running = true;
                _thread.Start();
            }
        }

        public void Stop()
        {
            lock (_lockObject)
            {
                _running = false;
            }
        }

        private void Charge()
        {

            while (true)
            {
                lock (_lockObject)
                {
                    if (!_running) break;
                }

                var sprucheLength = _database.GetSpruecheLength().GetAwaiter().GetResult();
                var (Header, Value) = _database.GetSpruchWithIndex(_random.Next(0, sprucheLength)).GetAwaiter().GetResult();

                try
                {
                    ConnectAsync().GetAwaiter().GetResult();
                    PlayAsync().GetAwaiter().GetResult();

                    var embedBuilder = new EmbedBuilder()
                        .WithThumbnailUrl("https://www.cigarworld.de/bilder/detail/small_500_265/2781_8717_41664.jpg")
                        .WithDescription("Hier gibt's was zum priesen!")
                        .WithColor(new Color(191, 89, 25))
                        .AddField("Überschrift", Header, false)
                        .AddField("Der Spruch", Value, false)
                        .WithCurrentTimestamp();

                    var embed = embedBuilder.Build();
                    _context.Message.Channel.SendMessageAsync(null, false, embed).GetAwaiter().GetResult();

                    LeaveAsync().GetAwaiter().GetResult();
                }
                catch {}

                Thread.Sleep(_random.Next(_time.Minimum, _time.Maximum));
            }
        }

        private async Task ConnectAsync()
        {
            if (_lavaNode.HasPlayer(_context.Guild)) return;

            var voiceState = _context.User as IVoiceState;
            if (voiceState?.VoiceChannel == null) return;

            try
            {
                await _lavaNode.JoinAsync(voiceState.VoiceChannel, _context.Channel as ITextChannel);
            }
            catch (Exception exception)
            {
                // TODO: Logging
            }
        }

        private async Task PlayAsync()
        {

            var videoLength = await _database.GetVideoLegth();
            var video = await _database.GetVideoWithIndex(_random.Next(0, videoLength));

    
            var searchResponse = await _lavaNode.SearchAsync(video);
            if (searchResponse.LoadStatus == LoadStatus.LoadFailed ||
                searchResponse.LoadStatus == LoadStatus.NoMatches)
                return;

            var player = _lavaNode.GetPlayer(_context.Guild);

            if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
                return;

            var track = searchResponse.Tracks[0];
            await player.PlayAsync(track);
        }

        private async Task LeaveAsync()
        {
            var player = _lavaNode.GetPlayer(_context.Guild);

            if (player == null) return;

            while (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused)
            {
                await Task.Delay(1000);
            }

            await _lavaNode.LeaveAsync(player.VoiceChannel);
        }

        public void Dispose()
        {
            if (_running)
                Stop();
        }
    }
}
