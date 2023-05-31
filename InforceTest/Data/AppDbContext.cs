using System.Collections.Generic;
using InforceTest.Models;
using Microsoft.EntityFrameworkCore;

namespace InforceTest.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public DbSet<Urls> URLs { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
    }
}
