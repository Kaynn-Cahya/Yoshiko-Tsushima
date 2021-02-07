namespace YoshikoDB {
    [System.Serializable]
    public enum ReminderType {
        Daily = 0,
        Weekly = 1,
        Monthly = 2,
        Quarterly = 3,
        PvP = 4, // Special case for Kancolle
        Training = 5, // Special case for LoveLive
    }
}
