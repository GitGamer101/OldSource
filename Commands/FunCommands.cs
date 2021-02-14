// Copyright [2021] [NotSoNitro]

using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace Lunar_Bot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("random")]
        [Aliases("guess")]
        [Description("A Fun Little Guess The Number Game!")]

        public async Task Guess(CommandContext ctx)
        {
            var interact = ctx.Client.GetInteractivity();

            var rand = new Random();

            int ans = rand.Next(1, 10);

            var embed = new DiscordEmbedBuilder
            {
                Title = "Random Number Guesser!",
                Description = "I Am Thinking Of A Number Between 1-10, What Number Am I Thinking Of? You Have 20 Seconds!",
                Color = DiscordColor.Azure
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            var msg = await interact.WaitForMessageAsync(x => x.ChannelId == ctx.Channel.Id && x.Author.Id == ctx.Member.Id, timeoutoverride: TimeSpan.FromSeconds(20)).ConfigureAwait(false);

            if (msg.TimedOut)
            {
                await ctx.RespondAsync("Times Up! You Failed To Choose A Number.").ConfigureAwait(false);
            }
            else
            {
                var guess = msg.Result.Content;

                int value;

                if (int.TryParse(guess, out value))
                    if (guess == ans.ToString())
                    {
                        var AnsEmbed = new DiscordEmbedBuilder
                        {
                            Title = "Correct! Well Done.",
                            Color = DiscordColor.SpringGreen
                        };

                        await ctx.Channel.SendMessageAsync(embed: AnsEmbed).ConfigureAwait(false);
                    }
                    else
                        await ctx.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder { Title = "Incorrect! Try Again Next Time.", Color = DiscordColor.Red}).ConfigureAwait(false);
                else
                    await ctx.Channel.SendMessageAsync(embed: new DiscordEmbedBuilder { Title = "`Argument Is Not Int Value.` Please Provide A Number.", Color = DiscordColor.DarkRed }).ConfigureAwait(false);
            }
        }

        [Command("8ball")]
        [Description("Ask The 8Ball Your Questions For Them To Be Answered")]

        public async Task Ball(CommandContext ctx, [Description("Your Question")][RemainingText]string question)
        {
            var rand = new Random();

            int randNum = rand.Next(4);

            switch (randNum)
            {
                case 0:
                    {
                        await ctx.Channel.SendMessageAsync("Yes");
                        break;
                    }
                case 1:
                    {
                        await ctx.Channel.SendMessageAsync("No");
                        break;
                    }
                case 2:
                    {
                        await ctx.Channel.SendMessageAsync("Maybe");
                        break;
                    }
                case 3:
                    {
                        await ctx.Channel.SendMessageAsync("Probably Not");
                        break;
                    }
                case 4:
                    {
                        await ctx.Channel.SendMessageAsync("I Dont Know");
                        break;
                    }
            }
        }
    }
}
