using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PriseBot.Helper;
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
        private ILogger _logger;

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
            _logger = workerInformation.Logger;
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

                ChargeHelper.Charge(_context, _lavaNode, _database, _logger).GetAwaiter().GetResult();

                var sleepTime = _random.Next(_time.Minimum, _time.Maximum);

                _logger.LogInformation($"Next Aufladen in {sleepTime / 1000}s.");

                Thread.Sleep(sleepTime);
            }
        }

        public void Dispose()
        {
            if (_running)
                Stop();
        }
    }
}
