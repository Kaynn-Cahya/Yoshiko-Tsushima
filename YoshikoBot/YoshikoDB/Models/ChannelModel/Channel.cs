using System.Collections.Generic;

namespace YoshikoDB.Models {
    public class Channel {

        public Channel(string channelID) {
            ChannelID = channelID;
            CustomReminders = new Dictionary<string, Reminder>();
            GameReminders = new Dictionary<string, bool>();
        }

        public string ChannelID { get; set; }

        public Dictionary<string, Reminder> CustomReminders { get; set; }

        public Dictionary<string, bool> GameReminders { get; set; }

        public void SetGameReminderState(GameReminder gameReminder, bool toggle) {
            string gameReminderStr = gameReminder.ToString();
            if (GameReminders.ContainsKey(gameReminderStr)) {
                GameReminders[gameReminderStr] = toggle;
            } else {
                GameReminders.Add(gameReminderStr, toggle);
            }
        }

        public bool GetGameReminderState(GameReminder gameReminder) {
            string gameReminderStr = gameReminder.ToString();
            if (!GameReminders.ContainsKey(gameReminderStr)) {
                return false;
            } else {
                return GameReminders[gameReminderStr];
            }
        }
    }
}
