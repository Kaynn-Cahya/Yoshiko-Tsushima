using System;

namespace YoshikoDB.Models {
    [System.Serializable]
#pragma warning disable CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
#pragma warning disable CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
    public struct GameReminder {
#pragma warning restore CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode()
#pragma warning restore CS0660 // Type defines operator == or operator != but does not override Object.Equals(object o)
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

        public override string ToString() {
            return $"{(int)GameType}-{(int)ReminderType}";
        }

        public static GameReminder FromString(string input) {
            // TODO: Exception handling
            string[] inputs = input.Split('-');
            return new GameReminder((GameType)Convert.ToInt32(inputs[0]), (ReminderType)Convert.ToInt32(inputs[1]));
        }
    }
}
