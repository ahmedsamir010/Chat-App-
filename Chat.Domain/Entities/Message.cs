using Chat.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Chat.Domain.Entities
{
    public class Message : BaseEntity
    {
        // Sender
        public string SenderId { get; set; } 
        public string SenderUserName { get; set; } = default!;

        public AppUser Sender { get; set; } = default!;

        // Recipent
        public string RecieptId { get; set; } 
        public string RecieptUserName { get; set; } = default!;
        public AppUser Recipient { get; set; } = default!;

        public string Content { get; set; } = default!;
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? DateRead { get; set; }
        public DateTime MessageSend { get; set; } = DateTime.Now;
           
        public bool  SenderDeleted{ get; set; }
        public bool  RecipientDeleted{ get; set; }

    }
}
