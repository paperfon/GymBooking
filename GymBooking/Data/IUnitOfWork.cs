using GymBooking.Core.Repositories;
using GymBooking.Data.Repositories;
using System.Threading.Tasks;

namespace GymBooking.Core
{
    public interface IUnitOfWork
    {
        IApplicationUserGymClassRepository UserGymClasses { get; }
        IGymClassesRepository GymClasses { get; }

        Task CompleteAsync();
    }
}