using Microsoft.AspNet.Identity.EntityFramework;
using Sklep_Internetowy.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Sklep_Internetowy.Infrastuctures
{
    public class ShopContext : IdentityDbContext<ApplicationUser>
    {
        public ShopContext()
            : base("SklepInternetowy")
        {

        }
        public static ShopContext Create()
        {
            return new ShopContext();
        }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Review> Reviews { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderIteam> OrderIteams { get; set; }
        public virtual DbSet<Contact> Contacts { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {

            var user = modelBuilder.Entity<ApplicationUser>().ToTable("AspNetUsers");
            user.Ignore(u => u.EmailConfirmed);
            user.Ignore(u => u.PhoneNumber);
            user.Ignore(u => u.PhoneNumberConfirmed);
            user.Ignore(u => u.TwoFactorEnabled);

            var identityUserRole = modelBuilder.Entity<IdentityUserRole>().ToTable("AspNetUserRoles");
            identityUserRole.HasKey(r => new { r.UserId, r.RoleId });
            var roles = modelBuilder.Entity<IdentityRole>().ToTable("AspNetRoles");
            var identityUserLogin = modelBuilder.Entity<IdentityUserLogin>().ToTable("AspNetUserLogins");
            identityUserLogin.HasKey(l => new { l.LoginProvider, l.ProviderKey, l.UserId });

            var claims = modelBuilder.Entity<IdentityUserClaim>().ToTable("AspNetUserClaims");

       
        }
    }
}