using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using PriseBot.Worker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriseBot.Modules
{
    [Group("prise")]
    public class GeneralModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<GeneralModule> _logger;
        private readonly Database _database;
        private readonly WorkerStore _workerStore;
        

        public GeneralModule(ILogger<GeneralModule> logger, Database database, WorkerStore workerStore)
        {
            _logger = logger;
            _database = database;
            _workerStore = workerStore;
        }

        [Command("start")]
        public async Task StartAsync()
        {
            _logger.LogInformation($"Command start was executed by {Context.User}");
            await _workerStore.AddAndStart<ChargeWorker>(new Records.WorkerInformation(Context.Guild.Id.ToString(), Context));
        }

        [Command("stop")]
        public async Task StopAsync()
        {
            _logger.LogInformation($"Command stop was executed by {Context.User}");
            await _workerStore.RemoveAndStop<ChargeWorker>(Context.Guild.Id.ToString());
        }
    }
}
