using Common.Models;
using Microsoft.EntityFrameworkCore;

namespace DataLayer.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class MyselfContext : DbContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MyselfContext(DbContextOptions<MyselfContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Task>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Entry>()
                .HasKey(c => new { c.Day, c.TaskId });

            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Task>()
                .Property(b => b.ModificationDate)
                .HasDefaultValueSql("getdate()");

            modelBuilder.Entity<Entry>()
                .Property(b => b.ModificationDate)
                .HasDefaultValueSql("getdate()");
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
