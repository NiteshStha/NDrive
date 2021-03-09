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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
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
        }
    }
}