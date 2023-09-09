using Discord.WebSocket;
using Discord;
using PeterBot.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PeterBot.Modules.Commands
{
    public class Whisper : ISlashCommand
    {
        public SlashCommandBuilder Build(DiscordSocketClient client)
        {
            var globalCommand = new SlashCommandBuilder()
                .WithName("whisper")
                .WithDescription("Whispers a message to the specified user")
                .AddOption("message", ApplicationCommandOptionType.String, "Message to be sent", isRequired: true)
                .AddOption("user", ApplicationCommandOptionType.User, "User to send the message to", isRequired: true);
            client.ButtonExecuted += InfoButtonHandler;
            return globalCommand;
        }

        public async Task Execute(SocketSlashCommand command)
        {
            string message = (string)command.Data.Options.ElementAt(0).Value;
            IUser user = (IUser)command.Data.Options.ElementAt(1).Value;

            var button = new ComponentBuilder()
                .WithButton("Secret Info", user.Id.ToString() + ":" + message).Build();

            IMessageChannel channel = await command.GetChannelAsync();
            await channel.SendMessageAsync("Secret Info Button", components: button);
        }

        public async Task InfoButtonHandler(SocketMessageComponent component)
        {
            string[] parts = component.Data.CustomId.Split(':');

            if (component.User.Id.ToString() == parts[0])
            {
                await component.RespondAsync(parts[1], ephemeral: true);
                return;
            }
            await component.RespondAsync("You aren't the recipient", ephemeral: true);
            return;
        }
    }
}
