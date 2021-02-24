using Discord.Commands;
using Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Victoria;

namespace PriseBot.Records
{
    public record WorkerInformation(string Guild, SocketCommandContext Context = null, MinMaxTime Time = null, LavaNode LavaNode = null, Database Database = null);
}
