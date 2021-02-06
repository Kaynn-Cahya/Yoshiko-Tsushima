namespace YoshikoDB.Models {
    [System.Serializable]
    public struct GameReminder {
        public GameType GameType { get; set; }

        public ReminderType ReminderType { get; set; }
    }
}
