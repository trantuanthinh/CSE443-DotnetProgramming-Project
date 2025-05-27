using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Models;
using Project.Utils;

namespace Project.AppContext
{
    public class DataContext(DbContextOptions<DataContext> options) : DbContext(options)
    {
        // Converters
        private readonly EnumToStringConverter<UserType> UserTypeConverter = new();
        private readonly EnumToStringConverter<LoginType> LoginTypeConverter = new();
        private readonly EnumToStringConverter<ItemStatus> ItemStatusConverter = new();

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<BorrowTransaction> BorrowTransactions { get; set; }
        public DbSet<Conversation> Conversations { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // o: orignal, d: destination
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Role).HasConversion(UserTypeConverter);
                entity.Property(e => e.LoginType).HasConversion(LoginTypeConverter);
            });

            modelBuilder.Entity<Item>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity
                    .HasOne(o => o.Category)
                    .WithMany()
                    .HasForeignKey(o => o.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.HasIndex(e => e.Name).IsUnique();
            });

            modelBuilder.Entity<BorrowTransaction>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Status).HasConversion(ItemStatusConverter);

                entity
                    .HasOne(o => o.Borrower)
                    .WithMany()
                    .HasForeignKey(o => o.BorrowerId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(o => o.Manager)
                    .WithMany()
                    .HasForeignKey(o => o.ManagerId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Conversation n - 1 FirstUser
                entity
                    .HasOne(o => o.FirstUser)
                    .WithMany()
                    .HasForeignKey(o => o.FirstUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Conversation n - 1 SecondaryUser
                entity
                    .HasOne(o => o.SecondUser)
                    .WithMany()
                    .HasForeignKey(o => o.SecondUserId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Conversation 1 - n Message
                entity
                    .HasMany(o => o.Messages)
                    .WithOne()
                    .HasForeignKey(o => o.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.HasKey(e => e.Id);

                // Message n - 1 Sender
                entity
                    .HasOne(o => o.Sender)
                    .WithMany()
                    .HasForeignKey(o => o.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
