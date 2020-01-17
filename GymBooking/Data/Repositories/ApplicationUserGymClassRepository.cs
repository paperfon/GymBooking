using GymBooking.Core.Repositories;
using GymBooking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.Data.Repositories
{
    public class ApplicationUserGymClassRepository : IApplicationUserGymClassRepository
    {
        private ApplicationDbContext context;

        public ApplicationUserGymClassRepository(ApplicationDbContext context)
        {
            this.context = context;
        }

        public void Add(ApplicationUserGymClass book)
        {
            context.ApplicationUserGymClass.Add(book);
        }

        public void Remove(ApplicationUserGymClass attending)
        {
            context.ApplicationUserGymClass.Remove(attending);
        }

        public async Task<List<GymClass>> GetAllBookings(string userId)
        {

            return await context.ApplicationUserGymClass
                .Where(augc => augc.ApplicationUserId == userId)
                .IgnoreQueryFilters()
                .Select(augc => augc.GymClass)
                .ToListAsync();
        }
    }
}
