using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GymBooking.Data;
using GymBooking.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using GymBooking.Data.Repositories;

namespace GymBooking.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            unitOfWork = new UnitOfWork(_context);
            this.userManager = userManager;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel vm = null)
        {
            if (vm.History)
            {
                List<GymClass> passesHistory = await unitOfWork.GymClasses.GetHistoryAsync();
                var model = new IndexViewModel { GymClasses = passesHistory };
                return View(model);
            }
            List<GymClass> passes = await unitOfWork.GymClasses.GetAllWithUsers();

            var model2 = new IndexViewModel { GymClasses = passes };

            return View(model2);
            //return View(await _context.GymClass.ToListAsync());
        }

        // GET: GymClasses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .Include(a => a.AttendingMembers)
                .ThenInclude(u => u.ApplicationUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // GET: GymClasses/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.GymClasses.Add(gymClass);
                await unitOfWork.CompleteAsync();
                //await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (id != gymClass.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(gymClass);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!unitOfWork.GymClasses.GymClassExists(gymClass.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await _context.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await _context.GymClass.FindAsync(id);
            _context.GymClass.Remove(gymClass);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: GymClasses/BookingToggle/5
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            //var userId = await _userManager.GetUserIdAsync(User);
            //var gymClass = await _context.GymClass.FindAsync(id);

            //User.Identity.

            //if (gymClass.AttendingMembers.Contains(userId))
            //{

            //}

            // Get the userid
            // var userId = _context.Users.FirstOrDefault(user => user.UserName == User.Identity.Name);
            //User.Identity.Name;
            var userId = userManager.GetUserId(User);
            GymClass currentGymClass = await unitOfWork.GymClasses.GetMembersAsync(id);

            // Is the user booked on the pass?
            var attending = currentGymClass.AttendingMembers.FirstOrDefault(u => u.ApplicationUserId == userId);

            // If not book the user on the pass
            if (attending == null)
            {
                var book = new ApplicationUserGymClass
                {
                    ApplicationUserId = userId,
                    GymClassId = currentGymClass.Id
                };
                unitOfWork.UserGymClasses.Add(book);
                //_context.ApplicationUserGymClass.Add(book);
                _context.SaveChanges();
            }

            // Otherwise unbook
            else
            {
                unitOfWork.UserGymClasses.Remove(attending);
                //_context.ApplicationUserGymClass.Remove(attending);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History()
        {
            var model = await _context.GymClass
                .Include(g => g.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .IgnoreQueryFilters()
                .Where(g => g.StartTime < DateTime.Now)
                .ToListAsync();

            return View(model);
            //return View(await _context.GymClass.ToListAsync());
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyBookedPasses()
        {
            var userId = userManager.GetUserId(User);
            List<GymClass> m = await unitOfWork.UserGymClasses.GetAllBookings(userId);

            //var m = await _context.ApplicationUserGymClass
            //    //.Include(a => a.ApplicationUser)
            //    //.Include(a => a.GymClass.AttendingMembers)
            //    .Where(a => a.ApplicationUserId == userId)
            //    .Select(a => a.GymClass)
            //    .ToListAsync();

            //var model2 = await _context.ApplicationUser
            //    .Include(a => a.AttendedClasses)
            //    .ThenInclude(g => g.GymClass)
            //    .Where(u => u.Id == userId)
            //    .ToListAsync();

            return View(m);

        }

        public async Task<IActionResult> MyHistoryBookedPasses()
        {
            var userId = userManager.GetUserId(User);

            var model = await _context.GymClass
                .Include(a => a.AttendingMembers)
                .ThenInclude(a => a.ApplicationUser)
                .IgnoreQueryFilters()
                .Where(a => a.AttendingMembers.All(x => x.ApplicationUserId == userId) && a.StartTime < DateTime.Now)
                .ToListAsync();

            return View(model);
        }
    }
}
