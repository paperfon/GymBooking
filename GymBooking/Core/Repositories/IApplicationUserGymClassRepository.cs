using GymBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBooking.Core.Repositories
{
    public interface IApplicationUserGymClassRepository
    {
        void Add(ApplicationUserGymClass book);
        Task<List<GymClass>> GetAllBookings(string userId);
        void Remove(ApplicationUserGymClass attending);
    }
}