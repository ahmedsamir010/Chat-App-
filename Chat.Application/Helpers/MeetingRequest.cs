using Microsoft.Extensions.Configuration;
using System;

namespace Chat.Application.Helpers
{
    public class MeetingRequest
    {
        public string? Topic { get; set; }
        public DateTime StartTime { get; set; }
      //  public int Duration { get; set; }
        //public string AccountID { get; set; } = "VNr81cIxSBCPoWzExQVnnQ";
        //public string ClientID { get; set; } = "B95gfnxAS7Gyx4rAWpiv1A";
        //public string ClientSecret { get; set; } = "psoRfbww33DFHM8lHpg3CB4cBmqQIGZD";
    }
}
