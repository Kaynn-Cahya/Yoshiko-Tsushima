using Discord;
using Discord.WebSocket;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

using Discord.Commands;

using YoshikoBot.Models;
using YoshikoBot.Persistence;

namespace YoshikoBot {
	public class Program {

        private string CredentialsFilePath {
            get { 
                return Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), "Credentials", "Token.json");
            }
        }

        private DiscordSocketClient client;

        private CommandHandler commandHandler;
        private CommandService commandService;

        public static void Main(string[] args)
			=> new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync() {
            client = new DiscordSocketClient();

            client.Log += Log;
            await SetupCommandModules();

            await ReminderTimers.Instance.Initalize(client);

            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(CredentialsFilePath)).Token;

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            await Task.Delay(-1);

            #region Local_Function
            async Task SetupCommandModules() {
                commandService = new CommandService();

                commandHandler = new CommandHandler(client, commandService);
                await commandHandler.InstallCommandsAsync();
            }
            #endregion
        }

        private Task Log(LogMessage msg) {
            Logger.Log(msg);
            return Task.CompletedTask;
        }
    }
}
