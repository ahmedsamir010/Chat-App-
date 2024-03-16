namespace Chat.Domain.Common
{
    public abstract class BaseEntity
    {
        public int Id { get; set; }
        public DateOnly CreatedDate => DateOnly.FromDateTime(DateTime.UtcNow);
        public DateOnly ModifiedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
