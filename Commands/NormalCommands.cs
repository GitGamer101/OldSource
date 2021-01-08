using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Threading.Tasks;

namespace Lunar_Bot.Commands
{
    public class NormalCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns Pong.")]

        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Pong! \n`Ping: {ctx.Client.Ping}ms`").ConfigureAwait(false);
        }
    }
}