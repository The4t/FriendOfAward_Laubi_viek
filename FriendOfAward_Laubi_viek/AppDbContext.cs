namespace FriendOfAward_Laubi_viek
{
    using Microsoft.EntityFrameworkCore;

    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<QRCode> QRCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<QRCode>()
                .HasIndex(q => q.Code)
                .IsUnique();
        }
    }

}
