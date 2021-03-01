using Discord.Commands;
using Infrastructure;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Records
{
    public record WorkerInformation(string Guild, SocketCommandContext Context = null, LavaNode LavaNode = null, MinMaxTime Time = null, Database Database = null, ILogger Logger = null);
}
