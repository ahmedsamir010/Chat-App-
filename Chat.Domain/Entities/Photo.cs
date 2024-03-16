using Chat.Domain.Common;

namespace Chat.Domain.Entities
{
    public class Photo : BaseEntity
    {

        public string Url { get; set; } = default!;
        public bool IsMain { get; set; } = default!;

        public string PuiblicId { get; set; } = string.Empty;

        public AppUser appUser { get; set; }

        public string AppUserId { get; set; } = default!;
    }
}