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
using GymBooking.Core;

namespace GymBooking.Controllers
{
    [Authorize]
    public class GymClassesController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly UserManager<ApplicationUser> userManager;

        public GymClassesController(UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork)
        {
            this.userManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        // GET: GymClasses
        [AllowAnonymous]
        public async Task<IActionResult> Index(IndexViewModel vm = null)
        {
            if (vm.History)
            {
                List<GymClass> passesHistory = await unitOfWork.GymClasses.GetHistoryAsync();
                var historyModel = new IndexViewModel { GymClasses = passesHistory };
                return View(historyModel);
            }
            List<GymClass> passes = await unitOfWork.GymClasses.GetAllClassesWithUsers();

            var model = new IndexViewModel { GymClasses = passes };

            return View(model);
        }

        // GET: GymClasses/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var gymClass = await unitOfWork.GymClasses.GetClassWithUsers(id);
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
            var gymClass = await unitOfWork.GymClasses.GetClassAsync(id);
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
                    unitOfWork.GymClasses.Update(gymClass);
                    await unitOfWork.CompleteAsync();
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

            var gymClass = await unitOfWork.GymClasses.GetClassAsync(id);
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
            var gymClass = await unitOfWork.GymClasses.GetClassAsync(id);
            unitOfWork.GymClasses.Remove(gymClass);
            await unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: GymClasses/BookingToggle/5
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null) return NotFound();

            // Get the userid
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
                await unitOfWork.CompleteAsync();
            }

            // Otherwise unbook
            else
            {
                unitOfWork.UserGymClasses.Remove(attending);
                await unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> History()
        {
            object model = await unitOfWork.GymClasses.GetHistoryAsync();

            return View(model);
        }

        [Authorize(Roles = "Member")]
        public async Task<IActionResult> MyBookedPasses()
        {
            var userId = userManager.GetUserId(User);
            List<GymClass> m = await unitOfWork.UserGymClasses.GetAllBookings(userId);

            return View(m);

        }

        public async Task<IActionResult> MyHistoryBookedPasses()
        {
            var userId = userManager.GetUserId(User);
            List<GymClass> model = await unitOfWork.GymClasses.MyHistoryBookedClasses(userId);

            return View(model);
        }

    }
}
