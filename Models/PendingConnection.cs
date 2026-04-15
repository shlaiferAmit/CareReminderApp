using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CareReminderApp.Models
{
    public class PendingConnection
    {
        public string Id { get; set; } = string.Empty;
        public string FamilyId { get; set; } = string.Empty;
        public string ElderId { get; set; } = string.Empty;
        public bool IsApproved { get; set; } = false;
        public bool IsRejected { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}