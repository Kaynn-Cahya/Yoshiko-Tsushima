using System;
using System.Collections.Generic;
using System.Text;

namespace YoshikoDB.Models {
    public class Channel {
        public string ChannelID { get; set; }

        public Dictionary<string, Reminder> CustomReminders { get; set; }

        public ArknightsReminders ArknightsReminders { get; set; }

        public GenshinImpactReminders GetGenshinImpactReminders { get; set; }

        public KancolleReminders KancolleReminders { get; set; }

        public LoveLive_AllStarsReminders LoveLive_AllStarsReminders { get; set; }
    }
}
