using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    // מחלקה זו מייצגת בקשת חיבור בין בן משפחה לקשיש במערכת
    // הבקשה נשמרת במסד הנתונים עד לאישור או דחייה
    public class PendingConnection
    {
        public string Id { get; set; } = string.Empty;

        public string FamilyId { get; set; } = string.Empty;

        public string ElderId { get; set; } = string.Empty;

        public bool IsApproved { get; set; } = false;

        public bool IsRejected { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string FamilyName { get; set; } = string.Empty;

    }
}