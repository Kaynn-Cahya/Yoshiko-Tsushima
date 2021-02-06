using System;
using System.Collections.Generic;
using System.Text;

namespace YoshikoDB.Models {
    /// <summary>
    /// Create this object for custom reminders;
    /// Otherwise, use pre-set reminders on channel models.
    /// </summary>
    public class Reminder {

        public DateTime NextRemindTime { get; set; }

        public string Message { get; set; }
    }
}
