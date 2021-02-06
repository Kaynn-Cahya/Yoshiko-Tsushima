using System.Collections.Generic;

namespace YoshikoDB.Models {
    public class Channel {

        public Channel(string channelID) {
            ChannelID = channelID;
            CustomReminders = new Dictionary<string, Reminder>();
            GameReminders = new Dictionary<GameReminder, bool>();
        }

        public string ChannelID { get; set; }

        public Dictionary<string, Reminder> CustomReminders { get; set; }

        public Dictionary<GameReminder, bool> GameReminders { get; set; }
    }
}
