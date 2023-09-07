using Discord;
using Discord.WebSocket;
using PeterBot.Services;

namespace PeterBot
{
    public class Program
    {
        private DiscordSocketClient _client;

        public static Task Main(string[] args) => new Program().MainAsync();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();

            _client.Log += Log;

            var token = "NDM3MDgwNjQ0MDgyOTkxMTI0.GHcYm1.ebdQkLhNWJUb1gapRqVDKQw8sNQqi4S7TcnODA";

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();
            SlashCommands commands = new SlashCommands(_client);

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