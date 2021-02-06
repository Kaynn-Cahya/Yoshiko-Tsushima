namespace YoshikoDB.Models {
    [System.Serializable]
    public struct GameReminder {
        public GameReminder(GameType gameType, ReminderType reminderType) {
            GameType = gameType;
            ReminderType = reminderType;
        }

        public GameType GameType { get; set; }

        public ReminderType ReminderType { get; set; }

        public static bool operator ==(GameReminder a, GameReminder b) {
            return a.GameType == b.GameType && a.ReminderType == b.ReminderType;
        }

        public static bool operator !=(GameReminder a, GameReminder b) {
            return !(a.GameType == b.GameType && a.ReminderType == b.ReminderType);
        }
    }
}
