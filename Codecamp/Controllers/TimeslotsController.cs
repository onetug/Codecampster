using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class TimeslotsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly ITimeslotBusinessLogic _timeslotBL;
        private readonly IEventBusinessLogic _eventBL;

        public TimeslotsController(CodecampDbContext context,
            ITimeslotBusinessLogic timeslotBL,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _timeslotBL = timeslotBL;
            _eventBL = eventBL;
        }

        // GET: Timeslots
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var timeslot = await _timeslotBL.GetAllTimeslotsForActiveEvent();

            return View(timeslot);
        }

        // GET: Timeslots/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var timeslot = await _timeslotBL.GetTimeslot(id.Value);
            if (timeslot == null)
            {
                return NotFound();
            }

            return View(timeslot);
        }

        // GET: Timeslots/Create
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Timeslots/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TimeslotId,StartTime,EndTime,ContainsNoSessions,Name,EventId")] Timeslot timeslot)
        {
            if (ModelState.IsValid)
            {
                var theEvent = await _eventBL.GetActiveEvent();
                if (theEvent != null)
                {
                    timeslot.EventId = theEvent.EventId;

                    await _timeslotBL.CreateTimeslot(timeslot);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(timeslot);
        }

        // GET: Timeslots/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var timeslot = await _timeslotBL.GetTimeslot(id.Value);
            if (timeslot == null)
            {
                return NotFound();
            }

            return View(timeslot);
        }

        // POST: Timeslots/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TimeslotId,StartTime,EndTime,ContainsNoSessions,Name,EventId")] Timeslot timeslot)
        {
            if (id != timeslot.TimeslotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _timeslotBL.UpdateTimeslot(timeslot) == false)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(timeslot);
        }

        // GET: Timeslots/Delete/5
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var timeslot = await _timeslotBL.GetTimeslot(id.Value);

            if (timeslot == null)
            {
                return NotFound();
            }

            return View(timeslot);
        }

        // POST: Timeslots/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Policy = "RequireAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _timeslotBL.DeleteTimeslot(id);

            return RedirectToAction(nameof(Index));
        }
    }
}