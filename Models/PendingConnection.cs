using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CareReminderApp.Models
{
    public class PendingConnection
    {
        public string Id { get; set; }
        public string FamilyId { get; set; }
        public string ElderId { get; set; }
        public bool IsApproved { get; set; } = false; // שדה חובה לניהול האישורים
        public bool IsRejected { get; set; } = false; // שדה חובה לניהול הדחיות
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}