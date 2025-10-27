using System.Reflection.Emit;
using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Finbuckle.MultiTenant.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Metadata.Conventions;
using MongoEntityFramework.AspNetCore.Identity;
using Net9MultiTenant.Models;

namespace Net9MultiTenant.Data
{
    public class ApplicationDbContext(IMultiTenantContextAccessor multiTenantContextAccessor,
        DbContextOptions<ApplicationDbContext> options) : MongoIdentityDbContext(options), IMultiTenantDbContext
    {

        public ITenantInfo? TenantInfo { get; } = multiTenantContextAccessor.MultiTenantContext.TenantInfo;
        public TenantMismatchMode TenantMismatchMode { get; set; } = TenantMismatchMode.Throw;
        public TenantNotSetMode TenantNotSetMode { get; set; } = TenantNotSetMode.Throw;


        public DbSet<ToDoItem> ToDoItems { get; set; }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Conventions.Add(_ => new CamelCaseElementNameConvention());
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            Database.AutoTransactionBehavior = AutoTransactionBehavior.Never;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // If necessary call the base class method.
            // Recommended to be called first.
            base.OnModelCreating(builder);
            builder.ConfigureMultiTenant();
            builder.Entity<MongoIdentityUser>().IsMultiTenant().AdjustUniqueIndexes();
            builder.Entity<MongoIdentityRole>().IsMultiTenant().AdjustUniqueIndexes();
            builder.Entity<ToDoItem>().IsMultiTenant();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            this.EnforceMultiTenant();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }

        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            this.EnforceMultiTenant();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
        }

    }
}
