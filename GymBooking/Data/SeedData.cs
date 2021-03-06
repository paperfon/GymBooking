﻿using GymBooking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.Data
{
    public static class SeedData
    {
        internal static async Task InitializeAsync(IServiceProvider services, string adminPW)
        {
            var options = services.GetRequiredService<DbContextOptions<ApplicationDbContext>>();
            using (var context = new ApplicationDbContext(options))
            {
                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                // Creating new roles
                var roleNames = new[] { "Admin", "Member" };

                foreach (var name in roleNames)
                {
                    // If the role is in the database just continue
                    if (await roleManager.RoleExistsAsync(name)) continue;

                    // Otherwise create the role
                    var role = new IdentityRole
                    {
                        Name = name
                    };
                    var result = await roleManager.CreateAsync(role);
                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                // Creating a new user
                var adminEmails = new[] { "admin@gym.se" };

                foreach (var email in adminEmails)
                {
                    var foundUser = await userManager.FindByEmailAsync(email);
                    if (foundUser != null) continue;

                    var user = new ApplicationUser
                    {
                        UserName = email,
                        Email = email,
                        TimeOfRegistration = DateTime.UtcNow
                    };
                    var result = await userManager.CreateAsync(user, adminPW);

                    if (!result.Succeeded)
                    {
                        throw new Exception(string.Join("\n", result.Errors));
                    }
                }

                // Assign the role to a user TODO: Refactor
                var adminUser = await userManager.FindByEmailAsync(adminEmails[0]);
                foreach (var role in roleNames)
                {
                    if (adminUser != null) continue;
                    // Assign a user to a role
                    var addToRoleResult = await userManager.AddToRoleAsync(adminUser, role);
                    if (!addToRoleResult.Succeeded)
                    {
                        throw new Exception(string.Join("\n", addToRoleResult.Errors));
                    }
                }


            }
        }
    }
}
