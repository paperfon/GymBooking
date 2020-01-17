using GymBooking.Core.Repositories;
using GymBooking.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymBooking.Data.Repositories
{
    public class GymClassesRepository : IGymClassesRepository
    {
        private readonly ApplicationDbContext _context;

        public GymClassesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<GymClass>> GetAllWithUsers()
        {
            return await _context.GymClass
                .Include(g => g.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .ToListAsync();
        }

        public async Task<List<GymClass>> GetHistoryAsync()
        {
            return await _context.GymClass
            .Include(g => g.AttendingMembers)
            .ThenInclude(a => a.ApplicationUser)
            .IgnoreQueryFilters()
            .Where(g => g.StartTime < DateTime.Now)
            .ToListAsync();
        }

        public async Task<GymClass> GetMembersAsync(int? id)
        {

            // Get the gym pass
            // Todo: remove button in ui if it's history
            return await _context.GymClass
                .Include(a => a.AttendingMembers)
                .FirstOrDefaultAsync(g => g.Id == id);
        }

        public bool GymClassExists(int id)
        {
            return _context.GymClass.Any(e => e.Id == id);
        }

        public void Add(GymClass gymClass)
        {

            _context.Add(gymClass);
        }
    }
}
