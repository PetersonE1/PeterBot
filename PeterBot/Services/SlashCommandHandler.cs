using Discord;
using Discord.Net;
using Discord.WebSocket;
using Newtonsoft.Json;
using PeterBot.Models;
using PeterBot.Modules.Commands;
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
        private readonly Type _thisType;
        private Dictionary<string, ISlashCommand> _commands = new Dictionary<string, ISlashCommand>();

        public SlashCommandHandler(DiscordSocketClient client, ConfigurationHandler configuration)
        {
            _client = client;
            _client.SlashCommandExecuted += HandleSlashCommandAsync;
            _configuration = configuration;
            _thisType = GetType();
            Build_Commands();
        }

        public void Register()
        {
            _client.Ready += Register_Commands;
        }

        public void Build_Commands()
        {
            foreach (string command in _configuration.Configuration.Commands)
            {
                Type? SlashCommand = Assembly.GetAssembly(_thisType)?.GetType($"PeterBot.Modules.Commands.{command}");
                if (SlashCommand == null) continue;

                ISlashCommand? SlashCommandInstance = SlashCommand.GetConstructor(new Type[0])?.Invoke(null) as ISlashCommand;
                if (SlashCommandInstance == null) continue;

                SlashCommandProperties properties = SlashCommandInstance.Build(_client).Build();
                _commands.Add(command.ToLower(), SlashCommandInstance);
                
            }
        }

        public async Task Register_Commands()
        {
            await _client.BulkOverwriteGlobalApplicationCommandsAsync(new ApplicationCommandProperties[0]);

            foreach (ISlashCommand command in _commands.Values)
            {
                SlashCommandProperties properties = command.Build(_client).Build();
                try
                {
                    await _client.CreateGlobalApplicationCommandAsync(properties);
                    Console.WriteLine($"Registered command {properties.Name}");
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
            if (_commands.TryGetValue(command.CommandName, out ISlashCommand? SlashCommand))
                await SlashCommand.Execute(command);
        }
    }
}
