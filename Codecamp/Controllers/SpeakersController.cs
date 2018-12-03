using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Codecamp.Data;
using Codecamp.Models;
using Microsoft.AspNetCore.Identity;
using Codecamp.BusinessLogic;
using Codecamp.ViewModels;
using System.IO;

namespace Codecamp.Controllers
{
    public class SpeakersController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly ISpeakerBusinessLogic _speakerBL;
        private readonly IUserBusinessLogic _userBL;

        private SpeakerViewModel Speaker { get; set; }

        public SpeakersController(
            UserManager<CodecampUser> userManager,
            CodecampDbContext context,
            ISpeakerBusinessLogic speakerBL,
            IUserBusinessLogic userBL)
        {
            _context = context;
            _userManager = userManager;
            _speakerBL = speakerBL;
            _userBL = userBL;
        }

        // GET: Speakers
        public async Task<IActionResult> Index()
        {
            List<Speaker> speakers;
            // Get the speakers for the active event. If there is
            // no active event, get all speakers for all events.
            if (User.IsInRole("Admin"))
            {
                speakers = await _speakerBL.GetAllSpeakersForActiveEvent();
            }
            else
            {
                speakers = await _speakerBL.GetAllApprovedSpeakersForActiveEvent();
            }

            // Get the current user, if the user exists.
            var user = await _userManager.GetUserAsync(User);
            ViewData["UserId"] = user != null ? user.Id : string.Empty;

            return View(speakers);
        }

        // GET: Speakers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // If no id specified, return not found
            if (id == null)
                return NotFound();

            // Find the specified speaker
            var speaker = await _speakerBL.GetSpeaker(id.Value);

            // If the speaker is not found, return not found
            if (speaker == null)
                return NotFound();

            // Else return the view with the speaker information
            return View(speaker);
        }

        // GET: Speakers/Create
        public IActionResult Create()
        {
            ViewData["CodecampUserId"] = new SelectList(_context.CodecampUsers, "Id", "Id");
            return View();
        }

        // POST: Speakers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SpeakerId,CompanyName,Bio,WebsiteUrl,BlogUrl,ImageUrl,NoteToOrganizers,IsMvp,LinkedIn,CodecampUserId")] Speaker speaker)
        {
            if (ModelState.IsValid)
            {
                _context.Add(speaker);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CodecampUserId"] = new SelectList(_context.CodecampUsers, "Id", "Id", speaker.CodecampUserId);
            return View(speaker);
        }

        // GET: Speakers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // If no id specified, return not found
            if (id == null)
                return NotFound();

            // Find the specified speaker
            var speaker = await _speakerBL.GetSpeaker(id.Value);

            // If the speaker is not found, return not found
            if (speaker == null)
                return NotFound();

            var user = await _speakerBL.GetUserInfoForSpeaker(speaker.SpeakerId);

            var speakerVM = new SpeakerViewModel
            {
                SpeakerId = speaker.SpeakerId,
                CodecampUserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CompanyName = speaker.CompanyName,
                Image = speaker.Image,
                Bio = speaker.Bio,
                WebsiteUrl = speaker.WebsiteUrl,
                BlogUrl = speaker.BlogUrl,
                GeographicLocation = user.GeographicLocation,
                TwitterHandle = user.TwitterHandle,
                IsVolunteer = user.IsVolunteer,
                IsMvp = speaker.IsMvp,
                NoteToOrganizers = speaker.NoteToOrganizers,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsApproved = speaker.IsApproved
            };

            // Else return the view with the speaker information
            return View(speakerVM);
        }

        // POST: Speakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("SpeakerId,CodecampUserId,FirstName,LastName,CompanyName,ImageFile,Image,Bio,WebsiteUrl,BlogUrl,GeographicLocation,TwitterHandle,LinkedIn,IsVolunteer,IsMvp,NoteToOrganizers,Email,PhoneNumber,IsApproved")] SpeakerViewModel speakerVM)
        {
            if (id != speakerVM.SpeakerId)
                return NotFound();

            if (ModelState.IsValid)
            {
                // Update the speaker information from the page
                var speaker = new Speaker
                {
                    SpeakerId = speakerVM.SpeakerId,
                    CompanyName = speakerVM.CompanyName,
                    Bio = speakerVM.Bio,
                    WebsiteUrl = speakerVM.WebsiteUrl,
                    BlogUrl = speakerVM.BlogUrl,
                    NoteToOrganizers = speakerVM.NoteToOrganizers,
                    IsApproved = speakerVM.IsApproved,
                    IsMvp = speakerVM.IsMvp,
                    LinkedIn = speakerVM.LinkedIn,
                    CodecampUserId = speakerVM.CodecampUserId,
                    Image = speakerVM.Image
                };

                // Convert the image to a byte array and store it in the
                // database
                if (speakerVM.ImageFile != null &&
                    speakerVM.ImageFile.ContentType.ToLower().StartsWith("image/"))
                {
                    MemoryStream ms = new MemoryStream();
                    speakerVM.ImageFile.OpenReadStream().CopyTo(ms);

                    speaker.Image = ms.ToArray();
                }

                var result = await _speakerBL.UpdateSpeaker(speaker);

                var user = await _userBL.GetUser(speakerVM.CodecampUserId);

                // Update the user information from the page
                user.FirstName = speakerVM.FirstName;
                user.LastName = speakerVM.LastName;
                user.GeographicLocation = speakerVM.GeographicLocation;
                user.TwitterHandle = speakerVM.TwitterHandle;
                user.IsVolunteer = speakerVM.IsVolunteer;
                user.Email = speakerVM.Email;
                user.PhoneNumber = speakerVM.PhoneNumber;

                result &= await _userBL.UpdateUser(user);

                if (result == false)
                    return NotFound();
                else
                    return RedirectToAction(nameof(Index));
            }

            return View(speakerVM);
        }

        // GET: Speakers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var speaker = await _context.Speakers
                .Include(s => s.CodecampUser)
                .FirstOrDefaultAsync(m => m.SpeakerId == id);
            if (speaker == null)
            {
                return NotFound();
            }

            return View(speaker);
        }

        // POST: Speakers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var speaker = await _context.Speakers.FindAsync(id);
            _context.Speakers.Remove(speaker);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
