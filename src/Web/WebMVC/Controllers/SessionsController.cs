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
using Microsoft.AspNetCore.Authorization;

namespace Codecamp.Controllers
{
    public class SessionsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly ISessionBusinessLogic _sessionBL;
        private readonly IEventBusinessLogic _eventBL;
        private readonly ISpeakerBusinessLogic _speakerBL;
        private readonly ITrackBusinessLogic _trackBL;
        private readonly ITimeslotBusinessLogic _timeslotBL;

        protected string UserId
        {
            get
            {
                // Get the user's Id
                return User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier) != null
                    ? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value
                    : null;
            }
        }

        public SessionsController(
            CodecampDbContext context,
            UserManager<CodecampUser> userManager,
            ISessionBusinessLogic sessionBL,
            IEventBusinessLogic eventBL,
            ISpeakerBusinessLogic speakerBL,
            ITrackBusinessLogic trackBL,
            ITimeslotBusinessLogic timeslotBL)
        {
            _context = context;
            _userManager = userManager;
            _sessionBL = sessionBL;
            _eventBL = eventBL;
            _speakerBL = speakerBL;
            _trackBL = trackBL;
            _timeslotBL = timeslotBL;
        }

        public class PageModel
        {
            public int SelectedUserType { get; set; }
            public int SelectedTrackId { get; set; }
            public int SelectedTimeslotId { get; set; }
            public bool ShowOnlyFavorites { get; set; }
            public List<UserType> UserTypes { get; set; }
            public List<SessionViewModel> Sessions { get; set; }
            public List<TrackViewModel> Tracks { get; set; }
            public List<TimeslotViewModel> Timeslots { get; set; }
        }

        public class DetailPageModel
        {
            public List<SpeakerViewModel> Speakers { get; set; }
            public SessionViewModel Session { get; set; }
        }

        // GET: Sessions
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var pageModel = new PageModel();
            pageModel.UserTypes = UserType.GetUserTypes().ToList();

            pageModel.Tracks = await _trackBL.GetAllTrackViewModelsForActiveEvent();
            pageModel.Tracks.Insert(0, new TrackViewModel { TrackId = 0, DisplayName = "All Tracks" });

            pageModel.Timeslots = await _timeslotBL.GetAllTimeslotViewModelsForActiveEvent();
            pageModel.Timeslots.Insert(0, new TimeslotViewModel { TimeslotId = 0, DisplayName = "All Timeslots" });

            if (User.IsInRole("Admin"))
            {
                // Get all sessions for the active event for the admin
                pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers; // JTL, I don't think this is actually necessary
                pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                ViewData["Title"] = "All Sessions";
            }
            else if (User.IsInRole("Speaker"))
            {
                pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers;

                var user = await _userManager.GetUserAsync(User);
                if (user != null && user.SpeakerId.HasValue)
                {
                    if (pageModel.SelectedUserType == (int)TypesOfUsers.SpecificUser)
                    {
                        // The user is a speaker and we have access to their speakerId, therefore
                        // get all of the speaker's sessions for the active event.
                        pageModel.Sessions = await _sessionBL.GetAllSessionViewModelsForSpeakerForActiveEvent(
                            user.SpeakerId.Value, UserId);

                        ViewData["Title"] = "Your Sessions";
                    }
                    else
                    {
                        // The user desires to see all approved sessions for the event
                        pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                        ViewData["Title"] = "Sessions";
                    }
                }
                else
                {
                    // We can't get the speakerId, so we'll return only approved
                    // speakers for the active event.
                    pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers;
                    pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                    ViewData["Title"] = "Sessions";
                }
            }
            else
            {
                // The user is an attendee, return all approved sessions for
                // the active event.
                pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers; // JTL, I don't think this is actually necessary
                pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                ViewData["Title"] = "Sessions";
            }

            pageModel.Sessions = pageModel.Sessions.OrderBy(s => s.StartTime).ToList();
            return View(pageModel);
        }

        // POST: Sessions/1
        [HttpPost]
        public async Task<IActionResult> Index([Bind("SelectedUserType, SelectedTrackId, SelectedTimeslotId, ShowOnlyFavorites")] PageModel pageModel)
        {
            pageModel.UserTypes = UserType.GetUserTypes().ToList();

            pageModel.Tracks = await _trackBL.GetAllTrackViewModelsForActiveEvent();
            pageModel.Tracks.Insert(0, new TrackViewModel { TrackId = 0, DisplayName = "All Tracks" });

            pageModel.Timeslots = await _timeslotBL.GetAllTimeslotViewModelsForActiveEvent();
            pageModel.Timeslots.Insert(0, new TimeslotViewModel { TimeslotId = 0, DisplayName = "All Timeslots" });

            if (User.IsInRole("Admin"))
            {
                // Get all sessions for the active event for the admin
                pageModel.Sessions = await _sessionBL.GetAllSessionViewModelsForActiveEvent(UserId);

                ViewData["Title"] = "All Sessions";
            }
            else if (User.IsInRole("Speaker"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null && user.SpeakerId.HasValue)
                {
                    if (pageModel.SelectedUserType == (int)TypesOfUsers.SpecificUser)
                    {
                        // The user is a speaker and we have access to their speakerId, therefore
                        // get all of the speaker's sessions for the active event.
                        pageModel.Sessions = await _sessionBL.GetAllSessionViewModelsForSpeakerForActiveEvent(
                            user.SpeakerId.Value, UserId);

                        ViewData["Title"] = "Your Sessions";
                    }
                    else
                    {
                        // The user desires to see all approved sessions for the event
                        pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                        ViewData["Title"] = "Sessions";
                    }
                }
                else
                {
                    // We can't get the speakerId, so we'll return only approved
                    // speakers for the active event.
                    pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers;
                    pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                    ViewData["Title"] = "Sessions";
                }
            }
            else
            {
                // The user is an attendee, return all approved sessions for
                // the active event.
                pageModel.SelectedUserType = (int)TypesOfUsers.AllUsers; // JTL, I don't think this is actually necessary
                pageModel.Sessions = await _sessionBL.GetAllApprovedSessionViewModelsForActiveEvent(UserId);

                ViewData["Title"] = "Sessions";
            }

            if (pageModel.SelectedTimeslotId > 0)
                pageModel.Sessions = pageModel.Sessions
                    .Where(s => s.TimeslotId == pageModel.SelectedTimeslotId)
                    .ToList();

            if (pageModel.SelectedTrackId > 0)
                pageModel.Sessions = pageModel.Sessions
                    .Where(s => s.TrackId == pageModel.SelectedTrackId)
                    .ToList();
            pageModel.Sessions = pageModel.Sessions.OrderBy(s => s.StartTime).ToList();

            return View(pageModel);
        }

        // GET: Sessions/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            if (!await _sessionBL.SessionExists(id.Value))
                return NotFound();

            var pageModel = new DetailPageModel();
            pageModel.Session = await _sessionBL.GetSessionViewModels(id.Value, UserId);

            if (pageModel.Session == null)
                return NotFound();

            pageModel.Speakers = await _speakerBL.GetSpeakersForSession(pageModel.Session.SessionId);

            return View(pageModel);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, Speaker")]
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
        [Authorize(Roles = "Admin, Speaker")]
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
        [Authorize(Roles = "Admin, Speaker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            var session = await _sessionBL.GetSession(id.Value);
            if (session == null)
                return NotFound();

            //If the user is not an Admin we need to do additional verification
            if (!User.IsInRole("Admin"))
            {
                // Get the user information
                var currentUser = await _userManager.GetUserAsync(User);

                var speaker = await _speakerBL.GetSpeaker(currentUser.SpeakerId.Value);
                //If the user is not the speaker for the session then they should not be able to edit it.
                if (!_sessionBL.IsSessionEditableBySpeaker(session.SessionId, speaker.SpeakerId))
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.SkillLevels = SkillLevel.GetSkillLevels();
            return View(session);
        }

        // POST: Sessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, Speaker")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SessionId,Name,Description,SkillLevel,Keywords,IsApproved,EventId")] Session session)
        {
            if (id != session.SessionId)
                return NotFound();

            if (ModelState.IsValid)
            {
                //If the user is not an Admin we need to do additional verification
                if (!User.IsInRole("Admin"))
                {
                    // Get the user information
                    var currentUser = await _userManager.GetUserAsync(User);

                    var speaker = await _speakerBL.GetSpeaker(currentUser.SpeakerId.Value);
                    //If the user is not the speaker for the session then they should not be able to edit it.
                    if (!_sessionBL.IsSessionEditableBySpeaker(session.SessionId, speaker.SpeakerId))
                    {
                        return RedirectToAction(nameof(Index));
                    }
                }

                if (await _sessionBL.UpdateSession(session) == false)
                    return NotFound();

                return RedirectToAction(nameof(Index));
            }

            return View(session);
        }

        // GET: Sessions/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
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
        [Authorize(Policy = "RequireAdminRole")]
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
