using Discord.Commands;
using Infrastructure;
using Infrastructure.Enities;
using Microsoft.Extensions.Logging;
using PriseBot.Helper;
using PriseBot.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Modules
{
    [Group("prise")]
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<GeneralModule> _logger;
        private readonly Database _database;
        private readonly WorkerStore _workerStore;
        private readonly LavaNode _lavaNode;

        public GeneralModule(ILogger<GeneralModule> logger, Database database, WorkerStore workerStore, LavaNode lavaNode)
        {
            _logger = logger;
            _database = database;
            _workerStore = workerStore;
            _lavaNode = lavaNode;
        }

        [Command("start")]
        public async Task StartAsync()
        {
            _logger.LogInformation($"Command start was executed by {Context.User}");
            await _workerStore.AddAndStart<ChargeWorker>(new Records.WorkerInformation(Context.Guild.Id.ToString(), Context, _lavaNode));
        }

        [Command("stop")]
        public async Task StopAsync()
        {
            _logger.LogInformation($"Command stop was executed by {Context.User}");
            await _workerStore.RemoveAndStop<ChargeWorker>(Context.Guild.Id.ToString());
        }

        [Command("aufladen")]
        public async Task ChargeOneTimeAsync()
        {
            _logger.LogInformation($"Command aufladen was executed by {Context.User}");
            await ChargeHelper.ChargeWithTTS(Context, _lavaNode, _database, _logger);
        }

        [Command("timesettings")]
        [Alias("time")]
        public async Task ChangeBotTimeAsync (int minTime, int maxTime)
        {
            if(minTime > maxTime)
            {
                await Context.Channel.SendMessageAsync($"Min time can't be more than max time.");
                return;
            }

            await _database.UpdateGuildSettings(Context.Guild.Id.ToString(), new GuildSettings(null, (minTime * 1000).ToString(), (maxTime * 1000).ToString()));
            await Context.Channel.SendMessageAsync($"Worker time has be changed to: Min = {minTime}s, Max = {maxTime}s");
            _logger.LogInformation($"Worker time for '{Context.Guild.Name}' has be changed to: Min = {minTime}s, Max = {maxTime}s");
        }
    }
}
