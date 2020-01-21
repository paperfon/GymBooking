using GymBooking.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymBooking.Core.Repositories
{
    public interface IGymClassesRepository
    {
        void Add(GymClass gymClass);
        void Update(GymClass gymClass);
        void Remove(GymClass gymClass);
        Task<List<GymClass>> GetAllClassesWithUsers();
        Task<List<GymClass>> GetHistoryAsync();
        Task<List<GymClass>> MyHistoryBookedClasses(string userId);
        Task<GymClass> GetMembersAsync(int? id);
        Task<GymClass> GetClassWithUsers(int? id);
        Task<GymClass> GetClassAsync(int? id);
        bool GymClassExists(int id);
        Task<IEnumerable<GymClass>> GetAllAsync();
    }
}