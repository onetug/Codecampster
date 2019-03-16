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
    public class TracksController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly ITrackBusinessLogic _trackBL;
        private readonly IEventBusinessLogic _eventBL;

        public TracksController(CodecampDbContext context,
            ITrackBusinessLogic trackBL,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _trackBL = trackBL;
            _eventBL = eventBL;
        }

        // GET: Tracks
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var track = await _trackBL.GetAllTracksForActiveEvent();

            return View(track);
        }

        // GET: Tracks/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var track = await _trackBL.GetTrack(id.Value);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // GET: Tracks/Create
        [HttpGet]
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Tracks/Create
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TrackId,Name,RoomNumber,EventId")] Track track)
        {
            if (ModelState.IsValid)
            {
                var theEvent = await _eventBL.GetActiveEvent();
                if (theEvent != null)
                {
                    track.EventId = theEvent.EventId;

                    await _trackBL.CreateTrack(track);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(track);
        }

        // GET: Tracks/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var track = await _trackBL.GetTrack(id.Value);
            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Edit/5
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TrackId,Name,RoomNumber,EventId")] Track track)
        {
            if (id != track.TrackId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                if (await _trackBL.UpdateTrack(track) == false)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(track);
        }

        // GET: Tracks/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
            {
                return NotFound();
            }

            var track = await _trackBL.GetTrack(id.Value);

            if (track == null)
            {
                return NotFound();
            }

            return View(track);
        }

        // POST: Tracks/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _trackBL.DeleteTrack(id);

            return RedirectToAction(nameof(Index));
        }
    }
}