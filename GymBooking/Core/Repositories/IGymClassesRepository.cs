using GymBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBooking.Core.Repositories
{
    public interface IGymClassesRepository
    {
        void Add(GymClass gymClass);
        Task<List<GymClass>> GetAllWithUsers();
        Task<List<GymClass>> GetHistoryAsync();
        Task<GymClass> GetMembersAsync(int? id);
        bool GymClassExists(int id);
    }
}