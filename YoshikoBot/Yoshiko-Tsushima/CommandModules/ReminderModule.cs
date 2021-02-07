using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

using YoshikoBot.Persistence;
using YoshikoDB.Models;
using YoshikoDB;
using System.Linq;

namespace YoshikoBot.CommandModules {
    public class ReminderModule : ModuleBase<SocketCommandContext> {

		[Command("setremind"), Alias("sr", "remindchannel")]
		public async Task SetPresetReminder([Remainder] string input) {
            if (Context.Channel as SocketGuildChannel == null) {
                await ReplyAsync("Only usable in server channels.");
                return;
            }

			input = input.ToLower();

            GameType inputGameType;
            if (!TryFindGameType(out inputGameType)) {
                await ReplyAsync("No valid game found.");
                return;
            }

            ReminderType inputReminderType;
            if (!TryFindReminderType(out inputReminderType)) {
                await ReplyAsync("No valid reminder type found.");
                return;
            }

            if (inputGameType != GameType.Kancolle && inputReminderType == ReminderType.PvP) {
                await ReplyAsync("This game doesn't support pvp reminder.");
                return;
            } else if (inputGameType != GameType.LoveLive_AllStars && inputReminderType == ReminderType.Training) {
                await ReplyAsync("This game doesn't support training reminder.");
                return;
            }

            GameReminder inputGameReminder = new GameReminder(inputGameType, inputReminderType);

            Query query = new Query();

            Channel currChannelData = await query.FetchChannelData(Context.Channel.Id.ToString());
            bool newRemindState = FlipChannelCurrentRemindState();

            await query.UpdateChannelData(currChannelData);

            if (newRemindState) {
                ReminderTimers.Instance.AddToSubscribedChannel(inputGameReminder, Context.Channel.Id.ToString());
                await ReplyAsync("Reminder for that has been turned on!");
            } else {
                ReminderTimers.Instance.RemoveFromSubscribedChannel(inputGameReminder, Context.Channel.Id.ToString());
                await ReplyAsync("Reminder for that has been turned off!");
            }

            #region Local_Function

            bool FlipChannelCurrentRemindState() {
                if (!currChannelData.GameReminders.ContainsKey(inputGameReminder)) {
                    currChannelData.GameReminders.Add(inputGameReminder, true);
                    return true;
                }

                bool currRemindState = currChannelData.GameReminders[inputGameReminder];
                currChannelData.GameReminders[inputGameReminder] = !currRemindState;

                return !currRemindState;
            }

            bool TryFindReminderType(out ReminderType reminderType) {
                string inputReminderName = input.Substring(input.LastIndexOf(' '), input.Length - input.LastIndexOf(' '));

                var reminderTypes = Enum.GetValues(typeof(ReminderType)).Cast<ReminderType>();

                foreach (ReminderType reminderName in reminderTypes) {
                    if (input.Contains(Enum.GetName(typeof(ReminderType), reminderName).ToLower())) {
                        reminderType = reminderName;
                        return true;
                    }
                }

                reminderType = ReminderType.Daily;
                return false;
            }

            bool TryFindGameType(out GameType gameType) {
                Dictionary<string, GameType> gameMatches = new Dictionary<string, GameType> {
                    { "kancolle" , GameType.Kancolle },
                    { "kc" , GameType.Kancolle },
                    { "kantai collection" , GameType.Kancolle },
                    { "arknight" , GameType.Arknights },
                    { "ak" , GameType.Arknights },
                    { "lovelive" , GameType.LoveLive_AllStars },
                    { "llas" , GameType.LoveLive_AllStars },
                    { "genshin" , GameType.Genshin_Impact }
                };

                string inputGameName = input.Substring(0, input.LastIndexOf(' '));

                var gameMatchesKey = gameMatches.Keys;
                foreach (var gameName in gameMatchesKey) {
                    if (inputGameName.Contains(gameName)) {
                        gameType = gameMatches[gameName];
                        return true;
                    }
                }

                gameType = GameType.Kancolle;
                return false;
            }

            #endregion
        }

    }
}
