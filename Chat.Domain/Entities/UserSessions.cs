// Chat.Domain.Entities.UserSession.cs
using System;

namespace Chat.Domain.Entities
{
    public class UserSession
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        // Navigation property for the user
        public AppUser User { get; set; }
    }
}
