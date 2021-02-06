using System;

namespace YoshikoDB {
    // TODO: All functions here should be in a class that is more documented.
    public class Utils {

        public static DateTime GetDateTimeNow_JST() {
            return DateTime.Now.ToUniversalTime().AddHours(9);
        }

        public static DateTime GetMonthlyResetTime(GameType gameType) {
            DateTime currDate = GetDateTimeNow_JST().AddMonths(1);
            return new DateTime(currDate.Year, currDate.Month, 1,
                0, 0, 0, currDate.Kind).AddHours(GetResetHourByGame(gameType));
        }

        public static DateTime GetWeeklyResetTime(GameType gameType) {
            DateTime currDate = GetDateTimeNow_JST();

            int diff = (7 + (currDate.DayOfWeek - DayOfWeek.Monday)) % 7;
            return currDate.AddDays(-1 * diff).Date.AddHours(GetResetHourByGame(gameType));
        }

        public static DateTime GetDailyResetTime(GameType gameType) {
            DateTime currDate = GetDateTimeNow_JST().AddDays(1);
            return currDate.AddHours(GetResetHourByGame(gameType));
        }

        private static int GetResetHourByGame(GameType gameType) {

            switch (gameType) {
                case GameType.ARKNIGHTS:
                    return 20; // JST 8PM
                case GameType.LOVELIVE_ALLSTARS:
                    return 23; //JST 11PM
            }

            // Genshin and kancolle
            return 5; // JST 5AM
        }
    }
}
