namespace YoshikoDB {
    [System.Serializable]
    public enum ReminderType {
        Daily,
        Weekly,
        Monthly,
        Quarterly,
        PvP, // Special case for Kancolle
        Training, // Special case for LoveLive
    }
}
