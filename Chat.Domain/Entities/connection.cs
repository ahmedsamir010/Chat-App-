using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat.Domain.Entities
{
    public class connection
    {
        public connection()
        {

        }
        public connection(string? connectionId, string? username)
        {
            ConnectionId = connectionId;
            Username = username;
        }

        public string? ConnectionId { get; set; }
        public string? Username { get; set; }
    }
}
