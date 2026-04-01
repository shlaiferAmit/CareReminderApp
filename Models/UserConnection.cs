using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    public class UserConnection
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string ConnectedUserId { get; set; } // למי הוא מחובר
    }
}
