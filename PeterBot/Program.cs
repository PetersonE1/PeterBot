using Discord;
using Discord.WebSocket;
using PeterBot.Services;

namespace PeterBot
{
    public class Program
    {
        private ConfigurationHandler _configuration;
        private DiscordSocketClient _client;
        private SlashCommandHandler _commands;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _configuration = new ConfigurationHandler("AppConfiguration.json");

            _client = new DiscordSocketClient();
            _client.Log += Log;

            _commands = new SlashCommandHandler(_client, _configuration);

            var token = _configuration.Configuration.Token;

            Console.WriteLine("1. Start with command registration\n2. Start without command registration");
            char startupOption;
            do
            {
                startupOption = Console.ReadKey().KeyChar;
            }
            while (startupOption != '1' && startupOption != '2');
            if (startupOption == '1')
                _commands.Register();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            Console.ReadLine();

            await _client.LogoutAsync();
            await _client.StopAsync();
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg);
            return Task.CompletedTask;
        }
    }
}