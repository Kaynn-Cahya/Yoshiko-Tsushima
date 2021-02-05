using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YoshikoBot {
    public class CommandHandler {
        private readonly DiscordSocketClient client;
        private readonly CommandService commands;

        // Retrieve client and CommandService instance via ctor
        public CommandHandler(DiscordSocketClient _client, CommandService _commands) {
            commands = _commands;
            client = _client;
        }

        public async Task InstallCommandsAsync() {
            client.MessageReceived += HandleCommandAsync;
            await commands.AddModulesAsync(assembly: Assembly.GetEntryAssembly(),
                                            services: null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam) {

            SocketUserMessage message = messageParam as SocketUserMessage;
            if (isSystemMessage()) {
                return;
            }

            int argPos = 0;

            if (!hasPrefixOrMention() || isBotMessage()) {
                return;
            }

            var context = new SocketCommandContext(client, message);

            await commands.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: null);

            #region Local_Function

            bool isSystemMessage() {
                return message == null;
            }

            bool isBotMessage() {
                return message.Author.IsBot;
            }

            bool hasPrefixOrMention() {
                return message.HasCharPrefix('>', ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos);
            }
            #endregion
        }
    }
}
