using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using PeterBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Services
{
    public class SlashCommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly ConfigurationHandler _configuration;

        public SlashCommandHandler(DiscordSocketClient client, ConfigurationHandler configuration)
        {
            _client = client;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
            _configuration = configuration;
        }

        public void Register()
        {
            _client.Ready += Build_Commands;
        }

        public async Task Build_Commands()
        {
            foreach (string command in _configuration.Configuration.Commands)
            {
                ISlashCommand? SlashCommand = Assembly.GetAssembly(GetType())?.GetType(command) as ISlashCommand;
                if (SlashCommand == null) continue;

                try
                {
                    await _client.CreateGlobalApplicationCommandAsync(SlashCommand.Build().Build());
                }
                catch (HttpException ex)
                {
                    var json = JsonConvert.SerializeObject(ex.Errors, Formatting.Indented);
                    Console.WriteLine(json);
                }
            }
        }

        private async Task HandleSlashCommandAsync(SocketSlashCommand command)
        {
            var message = command.Data.Options.First().Value as string;

            await command.RespondAsync(message);
        }
    }
}
