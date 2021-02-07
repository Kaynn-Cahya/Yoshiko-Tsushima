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
            client.Ready += Ready;
            await SetupCommandModules();

            await ReminderTimers.Instance.Initalize(client);

#if DEBUG
            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(CredentialsFilePath)).Debug_Token;
#else
            string token = JsonConvert.DeserializeObject<Credentials>(File.ReadAllText(CredentialsFilePath)).Token;
#endif

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

        private Task Ready() {

            Logger.Log(LogSeverity.Info, "Logged in as " + client.CurrentUser.Username);

            return Task.CompletedTask;
        }
        private Task Log(LogMessage msg) {
            Logger.Log(msg);
            return Task.CompletedTask;
        }
    }
}
