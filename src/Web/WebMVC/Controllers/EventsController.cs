using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.BusinessLogic;
using Microsoft.AspNetCore.Authorization;

namespace Codecamp.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class EventsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly IEventBusinessLogic _eventBL;

        public EventsController(CodecampDbContext context,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _eventBL = eventBL;
        }

        // GET: Events
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _eventBL.GetAllEvents());
        }

        // GET: Events/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var @event = await _eventBL.GetEvent(id.Value);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,Name,SocialMediaHashtag,StartDateTime,EndDateTime,LocationAddress,IsActive,IsAttendeeRegistrationOpen,IsSpeakerRegistrationOpen")] Event @event)
        {
            if (ModelState.IsValid)
            {
                await _eventBL.CreateEvent(@event);
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var @event = await _eventBL.GetEvent(id.Value);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,Name,SocialMediaHashtag,StartDateTime,EndDateTime,LocationAddress,IsActive,IsAttendeeRegistrationOpen,IsSpeakerRegistrationOpen")] Event @event)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _eventBL.UpdateEvent(@event) == false)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var theEvent = await _eventBL.GetEvent(id.Value);

            if (theEvent == null)
            {
                return NotFound();
            }

            return View(theEvent);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _eventBL.DeleteEvent(id);

            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
