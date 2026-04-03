using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareReminderApp.Models
{
    public partial class User : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public int RoleId { get; set; }
        public UserRole Role { get; set; }
    }

    public enum UserRole
    {
        Senior,
        FamilyMember
    }
}