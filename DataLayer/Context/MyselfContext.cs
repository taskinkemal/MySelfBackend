using Common;
using Common.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace DataLayer.Context
{
    /// <summary>
    /// 
    /// </summary>
    public class MyselfContext : DbContext
    {
        private readonly IOptions<AppSettings> settings;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public MyselfContext(DbContextOptions<MyselfContext> options, IOptions<AppSettings> settings) : base(options)
        {
            this.settings = settings;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var date = settings.Value.IsSqLite ? "datetime('now')" : "getdate()";

            modelBuilder.Entity<Task>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Task>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Entry>()
                .HasKey(c => new { c.Day, c.TaskId });

            modelBuilder.Entity<User>()
                .HasKey(c => c.Id);
            
            modelBuilder.Entity<UserToken>()
                .HasKey(c => new { c.UserId, c.DeviceId });

            modelBuilder.Entity<Task>()
                        .Property(b => b.ModificationDate)
                        .HasDefaultValueSql(date);

            modelBuilder.Entity<Entry>()
                        .Property(b => b.ModificationDate)
                        .HasDefaultValueSql(date);
        }

        public DbSet<Task> Tasks { get; set; }
        public DbSet<Entry> Entries { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }
    }
}
