using Microsoft.Extensions.Configuration;
using System;

namespace Chat.Application.Helpers
{
    public class MeetingRequest
    {
        public string? Topic { get; set; }
        public DateTime StartTime { get; set; }
        public int Duration { get; set; }
        public string AccountID { get; set; } = null!;
        public string ClientID { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
