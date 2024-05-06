using Microsoft.AspNetCore.Identity;
namespace Chat.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public string VerificationCode { get; set; } = "";
        public string FirstName { get; set; } = default!;
        public string LastName { get; set; } = default!;
        public DateTime DateOfBirth { get; set; }
        public string? KnownAs { get; set; }
        public DateTime LastActive { get; set; } = DateTime.Now;
        public DateTime Created { get; set; } = DateTime.Now;
        public string? Gender { get; set; }
        public string? Introduction { get; set; } = default!;
        public string? LookingFor { get; set; } = default!;
        public string? Interests { get; set; } = default!;
        public string? City { get; set; } =  default!;
        public string? Country { get; set; } = default!;
        public bool? IsBlocked { get; set; }
        public ICollection<Photo> Photos { get; set; } = new HashSet<Photo>();
        public ICollection<UserLike> Likeduser { get; set; } = new HashSet<UserLike>();  
        public ICollection<UserLike> LikedByUser { get; set; } = new HashSet<UserLike>();  
        public ICollection<Message> MessageSend { get; set; } = new HashSet<Message>();  
        public ICollection<Message> MessageRecived { get; set; } = new HashSet<Message>();
        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();
        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

    }
}
