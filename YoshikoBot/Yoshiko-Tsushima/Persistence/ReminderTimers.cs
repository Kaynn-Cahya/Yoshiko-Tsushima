using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using YoshikoDB.Models;
using YoshikoDB;

namespace YoshikoBot.Persistence {
    internal class ReminderTimers {

        private enum AlertTimings { 
            Two_Hour_Before,
            One_Hour_Before,
            Just_Occured
        }

        #region PresetTimers

        private class PresetTimer {
            public delegate Task AlertReminder(GameReminder gameReminder, AlertTimings alertTimings);

            public AlertReminder ToRemind { get; set; }

            private Timer timer;

            private AlertTimings currAlertTiming;

            public GameReminder GameReminder { get; private set; }

            public PresetTimer(GameReminder gameReminder) {
                GameReminder = gameReminder;
                SetNextAlertTiming();
            }

            private void SetNextAlertTiming() {
                DateTime nextReset = GetNextResetTime(GameReminder);
                DateTime timeNow = GetDateTimeNow_JST();

                int hourDiff = nextReset.Hour - timeNow.Hour;

                if (hourDiff > 2) {
                    currAlertTiming = AlertTimings.Two_Hour_Before;
                    nextReset.AddHours(-2);
                } else if (hourDiff > 1) {
                    currAlertTiming = AlertTimings.One_Hour_Before;
                    nextReset.AddHours(-1);
                } else {
                    currAlertTiming = AlertTimings.Just_Occured;
                }

                TimeSpan timeDiff = nextReset - timeNow;

                if (timer == null) {
                    timer = new Timer(Alert);
                }

                timer.Change(TimeSpan.Zero, timeDiff);
            }

            private void Alert(Object objInfo) {
                ToRemind?.Invoke(GameReminder, currAlertTiming);
                SetNextAlertTiming();
            }
        }

        #endregion

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

        /// <summary>
        /// Channels to remind.
        /// </summary>
        private Dictionary<GameReminder, HashSet<string>> subscribedChannels;

        private HashSet<PresetTimer> presetTimers;


        /// <summary>
        /// TODO
        /// </summary>
        private HashSet<Timer> customTimers;

        internal async Task Initalize(DiscordSocketClient client) {
            if (Initalized) {
                Logger.Log(Discord.LogSeverity.Warning, $"ReminderTimer got initalized again;");
            }

            clientRef = client;
            presetTimers = new HashSet<PresetTimer>();
            customTimers = new HashSet<Timer>();

            subscribedChannels = new Dictionary<GameReminder, HashSet<string>>();

            Query query = new Query();
            HashSet<Channel> allChannels = await query.FetchAllChannelsData();

            foreach (var channel in allChannels) {
                foreach (var gameToRemind in channel.GameReminders) {
                    if (gameToRemind.Value) {
                        AddToSubscribedChannel(gameToRemind.Key, channel.ChannelID);
                    }
                }
            }

            foreach (var gameReminder in subscribedChannels.Keys) {
                CreateGameTimerIfNotExists(gameReminder);
            }

            Initalized = true;
        }

        internal void AddToSubscribedChannel(GameReminder gameReminder, string channelID) {
            if (subscribedChannels.ContainsKey(gameReminder)) {
                subscribedChannels[gameReminder].Add(channelID);
            } else {
                HashSet<string> newSet = new HashSet<string>() { 
                    channelID
                };
                subscribedChannels.Add(gameReminder, newSet);
            }
        }

        internal void RemoveFromSubscribedChannel(GameReminder gameReminder, string channelID) {
            if (subscribedChannels.ContainsKey(gameReminder)) {
                subscribedChannels[gameReminder].Remove(channelID);
            }
        }

        private void CreateGameTimerIfNotExists(GameReminder gameReminder) {
            foreach (var presetTimer in presetTimers) {
                if (presetTimer.GameReminder == gameReminder) {
                    return;
                }
            }

            PresetTimer newTimer = new PresetTimer(gameReminder) {
                ToRemind = RemindChannels
            };
            presetTimers.Add(newTimer);
        }

        private Task RemindChannels(GameReminder gameReminder, AlertTimings alertTimings) {
            if (!subscribedChannels.ContainsKey(gameReminder)) {
                Logger.Log(Discord.LogSeverity.Error, $"Alert triggered but no channel list is created for it! @ {gameReminder}");
            }

            string alertMsg = CreateAlertMessage();

            HashSet<string> channelsToRemind = subscribedChannels[gameReminder];

            var remindTasks = new HashSet<Task>();
            foreach (var channelId in channelsToRemind) {
                remindTasks.Add(Task.Run(() => {
                    (clientRef.GetChannel(Convert.ToUInt64(channelId)) as SocketTextChannel).SendMessageAsync(alertMsg);
                }));
            }

            Task remindingTask = Task.WhenAll(remindTasks);
            remindingTask.Wait();

            return Task.CompletedTask;

            #region Local_Function

            string CreateAlertMessage() {
                StringBuilder msg = new StringBuilder();
                msg.Append(Enum.GetName(typeof(GameType), gameReminder.GameType).Replace("_", " ") + " ");
                msg.Append(Enum.GetName(typeof(ReminderType), gameReminder.ReminderType).ToLower() + " ");

                if (alertTimings == AlertTimings.Just_Occured) {
                    msg.Append("have been resetted!");
                } else if (alertTimings == AlertTimings.One_Hour_Before) {
                    msg.Append("will reset in 1 hour!");
                } else if (alertTimings == AlertTimings.Two_Hour_Before) {
                    msg.Append("will reset in 2 hour!");
                }

                return msg.ToString();
            }

            #endregion
        }

        #region Utils
        public static DateTime GetDateTimeNow_JST() {
            return DateTime.Now.ToUniversalTime().AddHours(9);
        }

        public static DateTime GetNextResetTime(GameReminder gameReminder) {

            switch (gameReminder.ReminderType) {
                case ReminderType.Daily:
                    return GetDailyResetTime(gameReminder.GameType);
                case ReminderType.Weekly:
                    return GetWeeklyResetTime(gameReminder.GameType);
                case ReminderType.Monthly:
                    return GetMonthlyResetTime(gameReminder.GameType);
                case ReminderType.Quarterly:
                    return GetQuarterlyResetTime(gameReminder.GameType);
                case ReminderType.PvP:
                    return GetPvPResetTiming();
                case ReminderType.Training:
                    return GetTrainingResetTiming();
            }

            Logger.Log(Discord.LogSeverity.Error, $"Couldn't find proper reminder type; @ ReminderTimer.cs;\r\n{gameReminder}");
            return GetDailyResetTime(gameReminder.GameType);

            #region Local_Function

            DateTime GetQuarterlyResetTime(GameType gameType) {
                DateTime currDate = GetDateTimeNow_JST().AddMonths(4);
                return new DateTime(currDate.Year, currDate.Month, 1,
                    0, 0, 0, currDate.Kind).AddHours(GetResetHourByGame(gameType));
            }

            DateTime GetMonthlyResetTime(GameType gameType) {
                DateTime currDate = GetDateTimeNow_JST().AddMonths(1);
                return new DateTime(currDate.Year, currDate.Month, 1,
                    0, 0, 0, currDate.Kind).AddHours(GetResetHourByGame(gameType));
            }

            DateTime GetWeeklyResetTime(GameType gameType) {
                DateTime currDate = GetDateTimeNow_JST();

                return new DateTime(currDate.Year, currDate.Month, currDate.Day + ((int)currDate.DayOfWeek + 1),
                    GetResetHourByGame(gameType), 0, 0, currDate.Kind);
            }

            DateTime GetDailyResetTime(GameType gameType) {
                DateTime currDate = GetDateTimeNow_JST().AddDays(1);

                return new DateTime(currDate.Year, currDate.Month, currDate.Day,
                    GetResetHourByGame(gameType), 0, 0, currDate.Kind);
            }

            DateTime GetPvPResetTiming() {
                DateTime currDate = GetDateTimeNow_JST();

                if (currDate.Hour >= 15) {
                    currDate.AddDays(1);

                    return new DateTime(currDate.Year, currDate.Month, currDate.Day,
                        3, 0, 0, currDate.Kind);
                } else {
                    return new DateTime(currDate.Year, currDate.Month, currDate.Day,
                        15, 0, 0, currDate.Kind);
                }
            }

            DateTime GetTrainingResetTiming() {
                DateTime currDate = GetDateTimeNow_JST();

                if (currDate.Hour >= 12) {
                    currDate.AddDays(1);

                    return new DateTime(currDate.Year, currDate.Month, currDate.Day,
                        0, 0, 0, currDate.Kind);
                } else {
                    return new DateTime(currDate.Year, currDate.Month, currDate.Day,
                        12, 0, 0, currDate.Kind);
                }
            }

            #endregion
        }

        private static int GetResetHourByGame(GameType gameType) {

            switch (gameType) {
                case GameType.Arknights:
                    return 20; // JST 8PM
                case GameType.LoveLive_AllStars:
                    return 24; //JST 12PM
            }

            // Genshin and kancolle
            return 5; // JST 5AM
        }
        #endregion
    }
}
