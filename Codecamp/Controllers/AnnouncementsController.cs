using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers
{
    [Authorize(Policy = "RequireAdminRole")]
    public class AnnouncementsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly IAnnouncementBusinessLogic _announcementBL;
        private readonly IEventBusinessLogic _eventBL;
        
        public AnnouncementsController(
            CodecampDbContext context,
            IAnnouncementBusinessLogic announcementBL,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _announcementBL = announcementBL;
            _eventBL = eventBL;
        }

        // GET: Announcements
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            List<AnnouncementViewModel> announcements;

            if (User.IsInRole("Admin"))
                announcements = await _announcementBL.GetAllAnnouncementsViewModelForActiveEvent();
            else
                announcements = await _announcementBL.GetActiveAnnouncementsViewModelForActiveEvent();

            return View(announcements);
        }

        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        // GET: Announcements/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [ValidateAntiForgeryToken]
        // POST: Announcements/Create
        public async Task<IActionResult> Create([Bind("AnnouncementId,Message,Rank,PublishOn,ExpiresOn")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                var theEvent = await _eventBL.GetActiveEvent();
                if (theEvent != null)
                {
                    announcement.EventId = theEvent.EventId;

                    await _announcementBL.CreateAnnouncement(announcement);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(announcement);
        }

        // GET: Announcements/Edit/5
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            if (!await _announcementBL.AnnouncementExists(id.Value))
                return NotFound();

            var announcement = await _announcementBL.GetAnnouncement(id.Value);
            if (announcement == null)
                return NotFound();

            return View(announcement);
        }

        // POST: Announcements/Edit/5
        [HttpPost]
        [Authorize(Policy = "RequireAdminRole")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("AnnouncementId,EventId,Message,Rank,PublishOn,ExpiresOn")] Announcement announcement)
        {
            if (ModelState.IsValid)
            {
                if (id != announcement.AnnouncementId)
                    return NotFound();

                var result = await _announcementBL.UpdateAnnouncement(announcement);

                if (result == false)
                    return NotFound();
                else
                    return RedirectToAction(nameof(Index));
            }

            return View(announcement);
        }

        // GET: Announcements/Delete/5
        [HttpGet]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            if (!await _announcementBL.AnnouncementExists(id.Value))
                return NotFound();

            var announcement = await _announcementBL.GetAnnouncement(id.Value);

            if (announcement == null)
                return NotFound();

            return View(announcement);
        }

        // POST: Announcements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _announcementBL.DeleteAnnouncement(id);

            return RedirectToAction(nameof(Index));
        }
    }
}