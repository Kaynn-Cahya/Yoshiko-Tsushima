using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using YoshikoBot.Models;

namespace YoshikoBot {
	public class Program {

        private string CredentialsFilePath {
            get { 
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Credentials", "Token.json");
            }
        }

        private DiscordSocketClient client;

        public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client = new DiscordSocketClient();

            client.Log += Log;

            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(CredentialsFilePath)).Token;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);
        }

        private Task Log(LogMessage msg) {
            Logger.Log(msg);
            return Task.CompletedTask;
        }
    }
}
