using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace YoshikoBot.Persistence {
    internal class ReminderTimers {

        #region Singleton
        private static ReminderTimers instance = null;
        private static readonly object padlock = new object();

        ReminderTimers() { }

        public static ReminderTimers Instance {
            get {
                lock (padlock) {
                    if (instance == null) {
                        instance = new ReminderTimers();
                    }
                    return instance;
                }
            }
        }
        #endregion

        public bool Initalized { get; private set; }
        private DiscordSocketClient clientRef;

        internal async Task Initalize() {
            if (Initalized) {
                Logger.Log(Discord.LogSeverity.Warning, $"ReminderTimer got initalized again;");
            }

            // TODO: setup timers based on database.

            Initalized = false;
        }
    }
}
