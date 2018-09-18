using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using IdentityChangeNames.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityChangeNames.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("User");

            builder.Entity<ApplicationRole>().ToTable("Role");

            builder.Entity<IdentityUserClaim<Guid>>().ToTable("UserClaim");

            builder.Entity<IdentityUserRole<Guid>>().ToTable("UserRole");

            builder.Entity<IdentityUserLogin<Guid>>().ToTable("UserLogin");

            builder.Entity<IdentityRoleClaim<Guid>>().ToTable("RoleClaim");

            builder.Entity<IdentityUserToken<Guid>>().ToTable("UserToken");

            builder.Entity<ApplicationUser>(b =>
            {
                b.Property(au => au.Email)
                .HasColumnName("EmailAddress");
            });
        }
    }
}
