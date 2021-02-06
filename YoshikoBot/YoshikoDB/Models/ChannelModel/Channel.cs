using System.Collections.Generic;

namespace YoshikoDB.Models {
    public class Channel {

        public Channel(string channelID) {
            this.ChannelID = channelID;
            this.CustomReminders = new Dictionary<string, Reminder>();
            this.ArknightsReminders = new ArknightsReminders();
            this.KancolleReminders = new KancolleReminders();
            this.GetGenshinImpactReminders = new GenshinImpactReminders();
            this.LoveLive_AllStarsReminders = new LoveLive_AllStarsReminders();
        }

        public string ChannelID { get; set; }

        public Dictionary<string, Reminder> CustomReminders { get; set; }

        public ArknightsReminders ArknightsReminders { get; set; }

        public GenshinImpactReminders GetGenshinImpactReminders { get; set; }

        public KancolleReminders KancolleReminders { get; set; }

        public LoveLive_AllStarsReminders LoveLive_AllStarsReminders { get; set; }
    }
}
