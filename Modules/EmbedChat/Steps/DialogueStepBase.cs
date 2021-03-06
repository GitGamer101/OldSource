// Copyright [2021] [NotSoNitro]

using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Lunar_Bot.Modules.EmbedChat.Steps
{
    public abstract class DialogueStepBase : IDialogueStep
    {
        protected readonly string _content;

        public DialogueStepBase(string content)
        {
            _content = content;
        }

        public Action<DiscordMessage> OnMessageAdded { get; set; } = delegate { };

        public abstract IDialogueStep NextStep { get; }

        public abstract Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);

        protected async Task TryAgain(DiscordChannel channel, string problem)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Something Went Wrong, Please Try Again.",
                Color = DiscordColor.DarkRed
            };

            embedBuilder.AddField("There was a problem with your previous input", problem);

            var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

            OnMessageAdded(embed);
        }
    }
}
