using Microsoft.EntityFrameworkCore;
using JapanApp.Models;

namespace JapanApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Festival> Festivals { get; set; }
        public DbSet<Region> Regions { get; set; }
        public DbSet<Season> Seasons { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<QuizQuestion> QuizQuestions { get; set; }
        public DbSet<QuizAnswer> QuizAnswers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Khóa chính b?ng Festival
            modelBuilder.Entity<Festival>()
                .HasKey(f => f.FestivalID);

            // Khóa chính b?ng Region
            modelBuilder.Entity<Region>()
                .HasKey(r => r.RegionID);

            // Khóa chính b?ng Season
            modelBuilder.Entity<Season>()
                .HasKey(s => s.SeasonID);

            // Khóa chính b?ng User
            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            // Festival - Region: 1 Region có nhi?u Festival
            modelBuilder.Entity<Festival>()
                .HasOne(f => f.Region)
                .WithMany(r => r.Festivals)
                .HasForeignKey(f => f.RegionID)
                .OnDelete(DeleteBehavior.Restrict);

            // Festival - Season: 1 Season có nhi?u Festival
            modelBuilder.Entity<Festival>()
                .HasOne(f => f.Season)
                .WithMany(s => s.Festivals)
                .HasForeignKey(f => f.SeasonID)
                .OnDelete(DeleteBehavior.Restrict);

            // Review - User: 1 User có nhi?u Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Review - Festival: 1 Festival có nhi?u Review
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Festival)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FestivalID)
                .OnDelete(DeleteBehavior.Cascade);

            // M?i user ch? ???c review 1 festival m?t l?n
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.UserID, r.FestivalID })
                .IsUnique();

            // Favorite dùng khóa chính kép
            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserID, f.FestivalID });

            // Favorite - User
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.User)
                .WithMany(u => u.Favorites)
                .HasForeignKey(f => f.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            // Favorite - Festival
            modelBuilder.Entity<Favorite>()
                .HasOne(f => f.Festival)
                .WithMany(fes => fes.Favorites)
                .HasForeignKey(f => f.FestivalID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}