using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Services
{
    public class SlashCommands
    {
        private readonly DiscordSocketClient _client;

        public SlashCommands(DiscordSocketClient client)
        {
            _client = client;
            _client.Ready += Client_Ready;
            _client.SlashCommandExecuted += SlashCommandHandler;
        }

        public async Task Client_Ready()
        {
            var globalCommand = new SlashCommandBuilder();
            globalCommand.WithName("first-global-command");
            globalCommand.WithDescription("Echoes back");
            globalCommand.AddOption("message", ApplicationCommandOptionType.String, "Message to be echoed", isRequired: true);

            try
            {
                await _client.CreateGlobalApplicationCommandAsync(globalCommand.Build());
            }
            catch (ApplicationCommandException ex)
            {
                var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                Console.WriteLine(json);
            }
        }

        private async Task SlashCommandHandler(SocketSlashCommand command)
        {
            var message = command.Data.Options.First().Value as string;

            await command.RespondAsync(message);
        }
    }
}
