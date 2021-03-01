using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Helper
{
    public static class ChargeHelper
    {

        private static readonly Random _random;

        static ChargeHelper()
        {
            _random = new Random();
        }

        public static async Task Charge(SocketCommandContext context, LavaNode lavaNode, Database database, ILogger logger)
        {
            var sprucheLength = database.GetSpruecheLength().GetAwaiter().GetResult();
            var (Header, Value) = database.GetSpruchWithIndex(_random.Next(0, sprucheLength)).GetAwaiter().GetResult();

            try
            {
                await LavaNodeHelper.ConnectAsync(context, lavaNode, logger);
                await LavaNodeHelper.PlayAsync(context, lavaNode, database);
                await LavaNodeHelper.BuildAndSendEmbed(context, (Header, Value));
                await LavaNodeHelper.LeaveAsync(context, lavaNode);
            }
            catch(Exception e)
            {
                logger?.LogInformation($"{nameof(Charge)} did not execute: {e.Message}");
            }
        }

        public static async Task ChargeWithTTS (SocketCommandContext context, LavaNode lavaNode, Database database, ILogger logger)
        {
            var sprucheLength = database.GetSpruecheLength().GetAwaiter().GetResult();
            var (Header, Value) = database.GetSpruchWithIndex(_random.Next(0, sprucheLength)).GetAwaiter().GetResult();

            try
            {
                await LavaNodeHelper.ConnectAsync(context, lavaNode, logger);
                await LavaNodeHelper.PlayAsync(context, lavaNode, database);
                await LavaNodeHelper.BuildAndSendEmbed(context, (Header, Value));
                await LavaNodeHelper.LeaveAsync(context, lavaNode);
                await Task.Delay(1000);
                await LavaNodeHelper.TTSAndDelete(context, (Header, Value));
            }
            catch (Exception e)
            {
                logger?.LogInformation($"{nameof(ChargeWithTTS)} did not execute: {e.Message}");
            }
        }
    }
}
