using Microsoft.EntityFrameworkCore;
using JapanApp.Models;

namespace JapanApp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

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

            // ================= PRIMARY KEYS =================

            modelBuilder.Entity<Festival>()
                .HasKey(f => f.FestivalID);

            modelBuilder.Entity<Region>()
                .HasKey(r => r.RegionID);

            modelBuilder.Entity<Season>()
                .HasKey(s => s.SeasonID);

            modelBuilder.Entity<User>()
                .HasKey(u => u.UserID);

            // ================= RELATIONSHIPS =================

            modelBuilder.Entity<Festival>()
                .HasOne(f => f.Region)
                .WithMany(r => r.Festivals)
                .HasForeignKey(f => f.RegionID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Festival>()
                .HasOne(f => f.Season)
                .WithMany(s => s.Festivals)
                .HasForeignKey(f => f.SeasonID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.User)
                .WithMany(u => u.Reviews)
                .HasForeignKey(r => r.UserID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Festival)
                .WithMany(f => f.Reviews)
                .HasForeignKey(r => r.FestivalID);

            modelBuilder.Entity<Favorite>()
                .HasKey(f => new { f.UserID, f.FestivalID });

            // ================= ADMIN SEED =================

            modelBuilder.Entity<User>().HasData(

                new User
                {
                    UserID = 1,
                    Username = "admin",
                    PasswordHash = "123",
                    Role = "Admin"
                },

                new User
                {
                    UserID = 2,
                    Username = "user",
                    PasswordHash = "123",
                    Role = "User"
                }
            );
        }
    }
}