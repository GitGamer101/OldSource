using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using DSharpPlus.Exceptions;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Lavalink;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Rem.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Rem
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public object SocketErrored { get; private set; }
        public EventId RemBotEventId { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            Client = new DiscordClient(config);

            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = false,
                CaseSensitive = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            Client.SocketErrored += Discord_SocketError;

            Client.GuildAvailable += Discord_GuildAvailable;

            Client.GuildCreated += Discord_GuildCreated;

            Client.GuildDownloadCompleted += Discord_GuildDownloadCompleted;

            Commands.CommandErrored += OnError;

            Commands.CommandExecuted += OnExecute;

            Commands.RegisterCommands<NormalCommands>();
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<AdminCommands>();
            Commands.RegisterCommands<BotOwnerCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }


        private Task Discord_GuildAvailable(DiscordClient client, GuildCreateEventArgs e)
        {
            client.Logger.LogInformation(RemBotEventId, "Guild available: '{0}'", e.Guild.Name);
            return Task.CompletedTask;
        }

        private Task Discord_GuildCreated(DiscordClient client, GuildCreateEventArgs e)
        {
            client.Logger.LogInformation(RemBotEventId, "Guild created: '{0}'", e.Guild.Name);
            return Task.CompletedTask;
        }

        private Task Discord_SocketError(DiscordClient client, SocketErrorEventArgs e)
        {
            var ex = e.Exception is AggregateException ae ? ae.InnerException : e.Exception;
            client.Logger.LogError(RemBotEventId, ex, "WebSocket threw an exception");
            return Task.CompletedTask;
        }

        private Task Discord_GuildDownloadCompleted(DiscordClient client, GuildDownloadCompletedEventArgs e)
        {
            client.Logger.LogDebug(RemBotEventId, "Guild download completed");
            return Task.CompletedTask;
        }

        private async Task OnError(object sender, CommandErrorEventArgs log)
        {
            log.Context.Client.Logger.Log(LogLevel.Error, $"[Rem]: {log.Context.User.Username} tried executing '{log.Command?.QualifiedName ?? "<unknown command>"}' but it errored: {log.Exception.GetType()}: {log.Exception.Message ?? "<no message>"}");

            if (log.Exception is ChecksFailedException)
            {
                await log.Context.RespondAsync("You Do Not Have Permission To Run This Command.").ConfigureAwait(false);
            }
            else if (log.Exception is ArgumentException)
            {
                await log.Context.RespondAsync($"Invalid Argument. \nMessage: `{log.Exception.Message}`").ConfigureAwait(false);
            }
            else if (log.Exception is MissingFieldException)
            {
                await log.Context.RespondAsync("Argument Does Not Exist.").ConfigureAwait(false);
            }
            else if (log.Exception is CommandNotFoundException)
            {
                await log.Context.RespondAsync("Invalid Command.").ConfigureAwait(false);
            }
            else if (log.Exception is BadRequestException)
            {
                await log.Context.RespondAsync($"Command Failed. \nError: `{log.Exception.Message}`").ConfigureAwait(false);
            }
            else if (log.Exception is UnauthorizedException)
            {
                await log.Context.RespondAsync($"I Am Unauthorized To Do That.").ConfigureAwait(false);
            }
        }

        private Task OnExecute(object sender, CommandExecutionEventArgs log)
        {
            log.Context.Client.Logger.LogInformation($"[Rem]: {log.Context.User.Username} successfully executed '{log.Command.QualifiedName}'");

            return Task.CompletedTask;
        }
    }
}
