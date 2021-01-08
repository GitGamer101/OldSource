using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Enums;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Lunar_Bot.Commands
{
    public class AdminCommands : BaseCommandModule
    {
        [Command("kick")]
        [RequireBotPermissions(Permissions.KickMembers)]
        [RequireUserPermissions(Permissions.KickMembers)]
        [Description("Kicks The Specified User.")]

        public async Task Kick(CommandContext ctx, [Description("The Specific Discord User")] DiscordMember user, [Description("The Reason For The Kick. Default: (You Have Been Kicked From The Server!)")][RemainingText] string reason = "You Have Been Kicked From The Server!")
        {
            await user.RemoveAsync($"`Kicked!` Reason: *{reason}*").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"User {user.DisplayName} Has Been Kicked!\nThere ID: `{user.Id}`").ConfigureAwait(false);
        }

        [Command("ban")]
        [RequireBotPermissions(Permissions.BanMembers)]
        [RequireUserPermissions(Permissions.BanMembers)]
        [Description("Bans The Specified User.")]

        public async Task Ban(CommandContext ctx, [Description("The Specific Discord User")] DiscordMember user, [Description("The Reason For The Ban. Default: (You Have Been Kicked From The Server!)")][RemainingText] string reason = "You Have Been Kicked From The Server!")
        {
            await user.BanAsync(0, $"`Banned!` Reason: *{reason}*").ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync($"User {user} Has Been Banned!\nThere ID: `{user.Id}`", false).ConfigureAwait(false);
        }

        private DiscordEmoji[] _pollEmojiCache;

        [Command("poll")]
        [Description("Creats A Poll.")]
        [RequireBotPermissions(Permissions.Administrator)]
        [RequireUserPermissions(Permissions.Administrator)]

        public async Task Poll(CommandContext ctx, [Description("Length Of Poll.")] TimeSpan time, [Description("Title For Poll.")][RemainingText]string title)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            var client = ctx.Client;

            if (_pollEmojiCache == null)
            {
                _pollEmojiCache = new[] {
                        DiscordEmoji.FromName(client, ":white_check_mark:"),
                        DiscordEmoji.FromName(client, ":heavy_minus_sign:"),
                        DiscordEmoji.FromName(client, ":x:")
                    };
            }

            var pollEmbed = new DiscordEmbedBuilder
            {
                Title = $"Poll Started!\n({title})",
                Description = $"\n{_pollEmojiCache[0]} = Yes, {_pollEmojiCache[1]} = I Dont Know, {_pollEmojiCache[2]} = No",
                Color = DiscordColor.LightGray
            };

            var pollMessage = await ctx.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            var pollResult = await interactivity.DoPollAsync(pollMessage, _pollEmojiCache, PollBehaviour.DeleteEmojis, time);

            Thread.Sleep(500);
            var yesVotes = pollResult[0].Total;
            var midVotes = pollResult[1].Total;
            var noVotes = pollResult[2].Total;

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Results For Poll ({title})",
                Description = $"{_pollEmojiCache[0]} Has A Total Of {yesVotes} Votes.\n{_pollEmojiCache[1]} Has A Total Of {midVotes} Votes.\n{_pollEmojiCache[2]} Has A Total Of {noVotes} Votes.",
                Color = DiscordColor.Green
            };

            await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            await pollMessage.DeleteAsync().ConfigureAwait(false);
        }

        [Command("purge")]
        [Description("Deleted A Certain Ammount Of Messages. (Max = 1000)")]
        [RequireBotPermissions(Permissions.Administrator)]
        [RequireUserPermissions(Permissions.Administrator)]

        public async Task Purge(CommandContext ctx, [Description("Ammount Of Messages To Be Deleted.")]int num)
        {
            int limit = 1000;
            
            if (num > limit)
                await ctx.Channel.SendMessageAsync("Too Many Arguments. Limit: 1000").ConfigureAwait(false);

            var messages = await ctx.Channel.GetMessagesAsync(num).ConfigureAwait(false);

            await ctx.Channel.DeleteMessagesAsync(messages).ConfigureAwait(false);

            var embed = new DiscordEmbedBuilder
            {
                Title = $"Deleted {num} Messages!",
                Color = DiscordColor.Aquamarine
            };
            var deletedMessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
            Thread.Sleep(4300);
            await ctx.Channel.DeleteMessageAsync(deletedMessage).ConfigureAwait(false);
        }
    }
}
