using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Chat.Application.Features.Accounts.Query.GetAllUsers
{
    public class MemberDto
    {
        public string Id { get; set; } 
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public int Age { get; set; }
        public string PhotoUrl { get; set; } = string.Empty;
        public string KnownAs { get; set; } = string.Empty;
        public DateTime LastActive { get; set; }
        public DateTime Created { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Introduction { get; set; } = string.Empty;
        public string LookingFor { get; set; } = string.Empty;
        public string Interests { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public ICollection<PhotoDto> Photos { get; set; } 
    }

    public class PhotoDto
    {
        public string Id { get; set; }  
        public string Url { get; set; } = string.Empty;
        public bool IsMain { get; set; }
    }
}
