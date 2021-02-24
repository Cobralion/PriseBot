using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using PriseBot.Records;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Worker
{
    public class WorkerStore
    {
        private readonly ILogger<WorkerStore> _logger;
        private readonly Dictionary<string, IWorker> _workers;
        private readonly LavaNode _lavaNode;
        private readonly Database _database;

        public WorkerStore(ILogger<WorkerStore> logger, Database database, LavaNode lavaNode)
        {
            _logger = logger;
            _database = database;
            _lavaNode = lavaNode;
            _workers = new Dictionary<string, IWorker>();
        }

        public async Task AddAndStart<T>(WorkerInformation workerInformation) where T : IWorker, new()
        {
            var name = $"{workerInformation.Guild}#{typeof(T)}";
            var t = new T();

            if (t is ChargeWorker)
            {
                var (Minimum, Maximum) = await _database.GetGuildSettings(workerInformation.Guild);

                var minMax = new MinMaxTime(Minimum, Maximum);

                t.Configure(workerInformation with { Time = minMax, LavaNode = _lavaNode, Database = _database });
            }

            if (_workers.ContainsKey(name)) _workers[name].Dispose();
            _workers.Add(name, t);

            t.Start();

            _logger.LogInformation($"Created and started new worker of type {typeof(T).FullName}");
        }

        public Task RemoveAndStop<T>(string guild) where T : IWorker, new()
        {
            var name = $"{guild}#{typeof(T)}";

            if (_workers.TryGetValue(name, out var worker))
            {
                worker.Dispose();
                _workers.Remove(name);
            }

            _logger.LogInformation($"Removed and stoped worker of type {typeof(T).FullName}");

            return Task.CompletedTask;
        }
    }
}
