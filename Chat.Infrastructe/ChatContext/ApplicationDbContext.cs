using Chat.Domain.Common;
using Chat.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Chat.Infrastructe.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
                .HasKey(ul => new { ul.SourceUserId, ul.LikedUserId });

            modelBuilder.Entity<UserLike>()
                .HasOne(ul => ul.SourceUser)
                .WithMany(u => u.Likeduser)
                .HasForeignKey(ul => ul.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction); // Adjust this behavior as needed

            modelBuilder.Entity<UserLike>()
                .HasOne(ul => ul.LikedUser)
                .WithMany(u => u.LikedByUser)
                .HasForeignKey(ul => ul.LikedUserId)
                .OnDelete(DeleteBehavior.NoAction); // Adjust this behavior as needed



            //Message

            modelBuilder.Entity<Message>()
                        .HasOne(x => x.Recipient)
                        .WithMany(x => x.MessageRecived)
                        .HasForeignKey(x => x.RecieptId)
                        .OnDelete(DeleteBehavior.NoAction);


            modelBuilder.Entity<Message>()
                        .HasOne(x => x.Sender)
                        .WithMany(x => x.MessageSend)
                        .HasForeignKey(x => x.SenderId)
                        .OnDelete(DeleteBehavior.NoAction);

            // user sessions 
            modelBuilder.Entity<UserSession>()
         .HasOne(us => us.User)
         .WithMany()
         .HasForeignKey(us => us.UserId);


        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Modified)
                    entry.Entity.ModifiedDate = DateOnly.FromDateTime(DateTime.UtcNow);
            }
            return base.SaveChangesAsync(cancellationToken);
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<UserLike> userLikes { get; set; }
        public DbSet<UserSession> UserSessions { get; set; }

        public DbSet<Group> Groups { get; set; }
        public DbSet<connection> Connections { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment>  Comments { get; set; }

    }

}
