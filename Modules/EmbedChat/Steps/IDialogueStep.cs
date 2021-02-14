// Copyright [2021] [NotSoNitro]

using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Threading.Tasks;

namespace Lunar_Bot.Modules.EmbedChat.Steps
{
    public interface IDialogueStep
    {
        Action<DiscordMessage> OnMessageAdded { get; set; }
        IDialogueStep NextStep { get; }
        Task<bool> ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user);
    }
}
