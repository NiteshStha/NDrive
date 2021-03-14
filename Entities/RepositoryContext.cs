using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using Utilities.Helpers;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
        }
        public DbSet<User> Users { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<Folder> Folders { get; set; }
        public DbSet<File> Files { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // For Cascade on delete when deleting the parent folder
            modelBuilder.Entity<Folder>()
              .HasMany(f => f.Folders)
              .WithOne()
              .OnDelete(DeleteBehavior.Cascade);

            // Making Email in User Table Unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            // Making Username in User Table Unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // Seeding User Table
            modelBuilder.Entity<User>().HasData(new
            {
                UserId = 1,
                FirstName = "Nitesh",
                LastName = "Shrestha",
                Username = "nitesh",
                Password = PasswordHasher.Hash("nitesh"),
                Email = "nitesh@gmail.com",
                DateOfBirth = new DateTime(1996, 11, 25),
                CreatedDate = DateTime.Now
            });

            // Seeding Folder Table
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 1,
                FolderName = "Game",
            });
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 2,
                FolderName = "Rocket League",
                ParentFolderId = 1
            });
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 3,
                FolderName = "Stardew Valley",
                ParentFolderId = 1
            });
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 4,
                FolderName = "Fortnite",
                ParentFolderId = 1
            });
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 5,
                FolderName = "Anime",
            });
            modelBuilder.Entity<Folder>().HasData(new
            {
                FolderId = 6,
                FolderName = "Attack On Titan",
                ParentFolderId = 5
            });

            // Seeding File Table
            modelBuilder.Entity<File>().HasData(new
            {
                FileId = 1,
                FileName = "Report.txt",
                FileLocation = "/Assets/Docs/Report.txt",
                FileExtension = "txt",
                FileSize = 5.0
            });
            modelBuilder.Entity<File>().HasData(new
            {
                FileId = 2,
                FileName = "Episode 1",
                FileLocation = "/Assets/Docs/Episode 1.mp4",
                FileExtension = "mp4",
                FileSize = 200.5,
                ParentFolderId = 6
            });
        }
    }
}