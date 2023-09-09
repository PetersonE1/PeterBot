using Discord.WebSocket;
using Discord;
using PeterBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;

namespace PeterBot.Modules.Commands
{
    public class Choose : ISlashCommand
    {
        public SlashCommandBuilder Build(DiscordSocketClient client)
        {
            var globalCommand = new SlashCommandBuilder()
                .WithName("choose")
                .WithDescription("Chooses a random item")
                .AddOption("items", ApplicationCommandOptionType.String, "Separate with semicolons", isRequired: true);
            return globalCommand;
        }

        public async Task Execute(SocketSlashCommand command)
        {
            string message = command.Data.Options.First().Value as string ?? string.Empty;
            string[] options = message.Split(';');

            if (options.Length == 0)
            {
                await command.RespondAsync("No options given");
                return;
            }

            int choice = RandomNumberGenerator.GetInt32(options.Length);
            string result = options[choice];
            if (result == null || result.Length == 0)
            {
                result = "null";
            }

            await command.RespondAsync(result);
        }
    }
}
