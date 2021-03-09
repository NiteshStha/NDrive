using Entities.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Helpers;

namespace Entities
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

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
                DateOfBirth = DateTime.Now
            });
        }
    }
}
