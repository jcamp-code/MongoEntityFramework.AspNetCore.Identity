using Microsoft.EntityFrameworkCore;

namespace MongoEntityFramework.AspNetCore.Identity.Tests.TestClasses
{
    public class MongoTestContext : DbContext
    {
        public MongoTestContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            this.Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        public DbSet<MongoTestUser> TestUsers { get; set; }
        public DbSet<MongoTestUserInt> TestUsersInt { get; set; }
        public DbSet<MongoIdentityRole> Roles { get; set; }
    }
}
