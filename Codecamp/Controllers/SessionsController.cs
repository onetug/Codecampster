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
using Microsoft.AspNetCore.Identity;
using Codecamp.ViewModels;

namespace Codecamp.Controllers
{
    public class SessionsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly ISessionBusinessLogic _sessionBL;
        private readonly IEventBusinessLogic _eventBL;
        private readonly ISpeakerBusinessLogic _speakerBL;

        public SessionsController(
            CodecampDbContext context,
            UserManager<CodecampUser> userManager,
            ISessionBusinessLogic sessionBL,
            IEventBusinessLogic eventBL,
            ISpeakerBusinessLogic speakerBL)
        {
            _context = context;
            _userManager = userManager;
            _sessionBL = sessionBL;
            _eventBL = eventBL;
            _speakerBL = speakerBL;
        }

        // GET: Sessions
        public async Task<IActionResult> Index()
        {
            List<SessionViewModel> sessions;
            if (User.IsInRole("Admin"))
            {
                // Get all sessions for the active event for the admin
                sessions = await _sessionBL.GetAllSessionsViewModelForActiveEvent();
            }
            else if (User.IsInRole("Speaker"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && user.SpeakerId.HasValue)
                    // The user is a speaker and we have access to their speakerId, therefore
                    // get all of the speaker's sessions for the active event.
                    sessions = await _sessionBL.GetAllSessionsViewModelForSpeakerForActiveEvent(
                        user.SpeakerId.Value);
                else
                    // We can't get the speakerId, s we'll return only approved
                    // speakers for the active event.
                    sessions = await _sessionBL.GetAllApprovedSessionsViewModelForActiveEvent();
            }
            else
            {
                // The user is an attendee, return all approved sessions for
                // the active event.
                sessions = await _sessionBL.GetAllApprovedSessionsViewModelForActiveEvent();
            }

            return View(sessions);
        }

        // GET: Sessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var session = await _sessionBL.GetSession(id.Value);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: Sessions/Create
        public IActionResult Create()
        {
            ViewBag.SkillLevels = SkillLevel.GetSkillLevels();
            return View();
        }

        // POST: Sessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SessionId,Name,Description,SkillLevel,Keywords")] Session session)
        {
            if (ModelState.IsValid)
            {
                var theEvent = await _eventBL.GetActiveEvent();
                if (theEvent != null)
                {
                    session.EventId = theEvent.EventId;

                    // Add the user to the list of speakers
                    Speaker speaker = null;
                    // Get the user information
                    var user = await _userManager.GetUserAsync(User);
                    // Does the user exist and are they a speaker?
                    if (user != null && user.SpeakerId.HasValue)
                        speaker = await _speakerBL.GetSpeaker(user.SpeakerId.Value);

                    // If the user is not a speaker, return to the speakers page
                    if (speaker == null)
                        return RedirectToAction(nameof(Index));

                    await _sessionBL.CreateSession(session, speaker.SpeakerId);

                    return RedirectToAction(nameof(Index));
                }
            }

            return View(session);
        }

        // GET: Sessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var session = await _sessionBL.GetSession(id.Value);
            if (session == null)
                return NotFound();

            ViewBag.SkillLevels = SkillLevel.GetSkillLevels();
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,Name,Description,SkillLevel,Keywords,IsApproved,EventId")] Session session)
        {
            if (id != session.SessionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                if (await _sessionBL.UpdateSession(session) == false)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(session);
        }

        // GET: Sessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var session = await _sessionBL.GetSession(id.Value);

            if (session == null)
                return NotFound();

            return View(session);
        }

        // POST: Sessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _sessionBL.DeleteSession(id);

            return RedirectToAction(nameof(Index));
        }

        private bool SessionExists(int id)
        {
            return _context.Sessions.Any(e => e.SessionId == id);
        }
    }
}
