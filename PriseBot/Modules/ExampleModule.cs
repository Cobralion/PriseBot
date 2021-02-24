using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriseBot.Modules
{
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        private readonly ILogger<ExampleModule> _logger;
        private readonly Database _database;

        public ExampleModule(ILogger<ExampleModule> logger, Database database)
        {
            _logger = logger;
            _database = database;
        }

        [Command("ping")]
        public async Task PingAsync()
        {
            await ReplyAsync("Pong!");
            _logger.LogInformation($"{Context.User.Username} executed the ping command!");
        }

        [Command("echo")]
        public async Task EchoAsync([Remainder] string text)
        {
            await ReplyAsync(text);
            _logger.LogInformation($"{Context.User.Username} executed the echo command!");
        }

        [Command("math")]
        public async Task MathAsync([Remainder] string math)
        {
            var dt = new DataTable();
            var result = dt.Compute(math, null);

            await ReplyAsync($"Result: {result}");
            _logger.LogInformation($"{Context.User.Username} executed the math command!");
        }
    }
}
