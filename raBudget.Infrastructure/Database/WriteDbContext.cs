using IdentityServer4.EntityFramework.Options;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using raBudget.Domain.Interfaces;
using raBudget.Domain.Models;

namespace raBudget.Infrastructure.Database
{
    public class WriteDbContext : ApiAuthorizationDbContext<ApplicationUser>, IWriteDbContext
    {
        public WriteDbContext(DbContextOptions options,IOptions<OperationalStoreOptions> operationalStoreOptions) : base(options, operationalStoreOptions)
        {
            
        }

        public DbSet<Budget> Budgets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ApplicationUser>(entity => {
                                                     entity.Property(m => m.Id).HasMaxLength(127);
                                                     entity.Property(m => m.Email).HasMaxLength(127);
                                                     entity.Property(m => m.NormalizedEmail).HasMaxLength(127);
                                                     entity.Property(m => m.NormalizedUserName).HasMaxLength(127);
                                                     entity.Property(m => m.UserName).HasMaxLength(127);
                                                 });
            modelBuilder.Entity<IdentityRole>(entity => {
                                                     entity.Property(m => m.Id).HasMaxLength(127);
                                                     entity.Property(m => m.Name).HasMaxLength(127);
                                                     entity.Property(m => m.NormalizedName).HasMaxLength(127);
                                                  entity.Property(m => m.ConcurrencyStamp).HasMaxLength(127);
                                                 });
            modelBuilder.Entity<ApplicationRole>(entity => {
                                                  entity.Property(m => m.Id).HasMaxLength(127);
                                                  entity.Property(m => m.Name).HasMaxLength(127);
                                                  entity.Property(m => m.NormalizedName).HasMaxLength(127);
                                                  entity.Property(m => m.ConcurrencyStamp).HasMaxLength(127);
                                              });
            modelBuilder.Entity<IdentityUserLogin<string>>(entity =>
                                                           {
                                                               entity.Property(m => m.LoginProvider).HasMaxLength(127);
                                                               entity.Property(m => m.ProviderKey).HasMaxLength(127);
                                                           });
            modelBuilder.Entity<IdentityUserRole<string>>(entity =>
                                                          {
                                                              entity.Property(m => m.UserId).HasMaxLength(127);
                                                              entity.Property(m => m.RoleId).HasMaxLength(127);
                                                          });
            modelBuilder.Entity<IdentityUserToken<string>>(entity =>
                                                           {
                                                               entity.Property(m => m.UserId).HasMaxLength(127);
                                                               entity.Property(m => m.LoginProvider).HasMaxLength(127);
                                                               entity.Property(m => m.Name).HasMaxLength(127);
                                                           });


            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WriteDbContext).Assembly, type => type.FullName != null && type.FullName.Contains("Configuration.Write"));
        }
    }
}