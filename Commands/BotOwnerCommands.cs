// Copyright [2021] [NotSoNitro]

using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Lunar_Bot.Modules.EmbedChat;
using Lunar_Bot.Modules.EmbedChat.Steps;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Lunar_Bot.Commands
{
    [RequireOwner]
    public class BotOwnerCommands : BaseCommandModule
    {

        [Command("say")]
        [Description("Repeats What You Say.")]

        public async Task Say(CommandContext ctx, [Description("Text To Repeat.")][RemainingText] string text)
        {
            await ctx.Channel.SendMessageAsync(text).ConfigureAwait(false);
        }

        [Command("sudo")]
        [Description("Execute A Command As Another User")]

        public async Task SudoAsync(CommandContext ctx, [Description("User/Id")]DiscordUser user, [Description("Text To Say")][RemainingText]string text)
        {
            var cmd = ctx.CommandsNext.FindCommand(text, out var args);
            var fctx = ctx.CommandsNext.CreateFakeContext(user, ctx.Channel, text, ctx.Prefix, cmd, args);
            await ctx.CommandsNext.ExecuteCommandAsync(fctx).ConfigureAwait(false);
        }

        [Command("enablechat")]
        [Hidden]
        public async Task Chat(CommandContext ctx)
        {
            await ctx.Message.DeleteAsync().ConfigureAwait(false);

            var inputStep = new StringStep("Enter You Text Below To Start Speaking.", null);
            var emptyStep = new StringStep("Please Enter Some Text Please.", null);
            var continueStep = new StringStep("Continue Chat By Typing Below.", null);
                
            string input = string.Empty;

            inputStep.OnValidResult += (result) =>
            {
                input = result;
            };

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
            
            var inputDialogueHandler = new EmbedChatHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                inputStep
            );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeeded) { return; }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);
        }

        [Command("changestatus")]
        [Aliases("cs", "change", "status")]
        [Description("Change The Status And Activity Of The Bot")]

        public async Task Status(CommandContext ctx, [Description("The Status Of The Bot.")]string state)
        {
            if (state.ToLower() == "online")
            {
                var status = UserStatus.Online;
                await ctx.Client.UpdateStatusAsync(userStatus: status).ConfigureAwait(false);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Status Changed To: `{state.ToUpper()}`",
                    Color = DiscordColor.SpringGreen
                };
                var deletemessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                Thread.Sleep(10000);
                await ctx.Channel.DeleteMessageAsync(deletemessage).ConfigureAwait(false);
            }
            else if (state.ToLower() == "idle")
            {
                var status = UserStatus.Idle;
                await ctx.Client.UpdateStatusAsync(userStatus: status).ConfigureAwait(false);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Status Changed To: `{state.ToUpper()}`",
                    Color = DiscordColor.Orange
                };
                var deletemessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                Thread.Sleep(10000);
                await ctx.Channel.DeleteMessageAsync(deletemessage).ConfigureAwait(false);
            }
            else if (state.ToLower() == "dnd" || state.ToLower() == "donotdisturb")
            {
                var status = UserStatus.DoNotDisturb;
                await ctx.Client.UpdateStatusAsync(userStatus: status).ConfigureAwait(false);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Status Changed To: `DO NOT DISTURB`",
                    Color = DiscordColor.Red
                };
                var deletemessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                Thread.Sleep(10000);
                await ctx.Channel.DeleteMessageAsync(deletemessage).ConfigureAwait(false);
            }
            else if (state.ToLower() == "invisible" || state.ToLower() == "invis")
            {
                var status = UserStatus.Invisible;
                await ctx.Client.UpdateStatusAsync(userStatus: status).ConfigureAwait(false);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Status Changed To: `INVISIBLE`",
                    Color = DiscordColor.LightGray
                };
                var deletemessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                Thread.Sleep(10000);
                await ctx.Channel.DeleteMessageAsync(deletemessage).ConfigureAwait(false);
            }
            else if (state.ToLower() == "offline")
            {
                var status = UserStatus.Offline;
                await ctx.Client.UpdateStatusAsync(userStatus: status).ConfigureAwait(false);
                var embed = new DiscordEmbedBuilder
                {
                    Title = $"Status Changed To: `{state.ToUpper()}`",
                    Color = DiscordColor.VeryDarkGray
                };
                var deletemessage = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);
                Thread.Sleep(10000);
                await ctx.Channel.DeleteMessageAsync(deletemessage).ConfigureAwait(false);
            }


        }
    }
}
