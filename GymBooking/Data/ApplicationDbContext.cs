using System;
using System.Collections.Generic;
using System.Text;
using GymBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GymBooking.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // Körs alltid först annars skrivs vår egen konfiguration över av default inställningarna i base
            base.OnModelCreating(builder);

            // Define a composite key
            builder.Entity<ApplicationUserGymClass>()
                .HasKey(t => new { 
                    t.ApplicationUserId, 
                    t.GymClassId 
                });

            builder.Entity<GymClass>().HasQueryFilter(g => g.StartTime > DateTime.Now);

            //builder.Entity<ApplicationUser>()
            //    .HasData(
            //        new ApplicationUser
            //        {

            //        }
            //    );
        }

        public DbSet<GymClass> GymClass { get; set; }
        public DbSet<ApplicationUser> ApplicationUser { get; set; }
        public DbSet<ApplicationUserGymClass> ApplicationUserGymClass { get; set; }
    }
}
