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
using System.ComponentModel.DataAnnotations;

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

        /// <summary>
        /// Perform validation on filesize
        /// </summary>
        public class FileSizeValidationAttribute : ValidationAttribute
        {
            private int _maxFileSize;

            public FileSizeValidationAttribute(int maxFileSize)
            {
                _maxFileSize = maxFileSize;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                var imageFile = ((SpeakerViewModel)validationContext.ObjectInstance).ImageFile;

                if (imageFile != null && imageFile.Length > _maxFileSize)
                {
                    return new ValidationResult(string.Format("File size limit is {0} KB", (_maxFileSize / 1000)));
                }

                return ValidationResult.Success;
            }
        }

        // GET: Speakers
        public async Task<IActionResult> Index()
        {
            List<SpeakerViewModel> speakers;

            // Get the speakers for the active event. If there is
            // no active event, get all speakers for all events.
            if (User.IsInRole("Admin"))
            {
                ViewData["Title"] = "All Speakers";
                // Don't load any images, this is list on the Admin version of this
                // page.
                speakers = await _speakerBL.GetAllSpeakersViewModelForActiveEvent(false);
            }
            else
            {
                ViewData["Title"] = "Speakers";
                speakers = await _speakerBL.GetAllApprovedSpeakersViewModelForActiveEvent();
            }

            return View(speakers);
        }

        // GET: Speakers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // If no id specified, return not found
            if (id == null)
                return NotFound();

            // Find the specified speaker
            var speaker = await _speakerBL.GetSpeakerViewModel(id.Value);

            // If the speaker is not found, return not found
            if (speaker == null)
                return NotFound();

            // Else return the view with the speaker information
            return View(speaker);
        }

        #region NOT USED - Create done with user registration

        //// GET: Speakers/Create
        //public IActionResult Create()
        //{
        //    ViewData["CodecampUserId"] = new SelectList(_context.CodecampUsers, "Id", "Id");
        //    return View();
        //}

        //// POST: Speakers/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("SpeakerId,CompanyName,Bio,WebsiteUrl,BlogUrl,ImageUrl,NoteToOrganizers,IsMvp,LinkedIn,CodecampUserId")] Speaker speaker)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(speaker);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CodecampUserId"] = new SelectList(_context.CodecampUsers, "Id", "Id", speaker.CodecampUserId);
        //    return View(speaker);
        //}

        #endregion

        // GET: Speakers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            // If no id specified, return not found
            if (id == null)
                return NotFound();

            // Find the specified speaker
            var speaker = await _speakerBL.GetSpeakerViewModel(id.Value);

            // If the speaker is not found, return not found
            if (speaker == null)
                return NotFound();

            var user = await _speakerBL.GetUserInfoForSpeaker(speaker.SpeakerId);

            // Else return the view with the speaker information
            return View(speaker);
        }

        // POST: Speakers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, 
            [Bind("SpeakerId,CodecampUserId,FirstName,LastName,CompanyName,ImageFile,Bio,WebsiteUrl,BlogUrl,GeographicLocation,TwitterHandle,LinkedIn,IsVolunteer,IsMvp,NoteToOrganizers,Email,PhoneNumber,IsApproved")] SpeakerViewModel speakerVM)
        {
            if (ModelState.IsValid)
            {
                if (id != speakerVM.SpeakerId)
                    return NotFound();

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
                };

                // Convert the image to a byte array and store it in the
                // database
                if (speakerVM.ImageFile != null &&
                    speakerVM.ImageFile.ContentType.ToLower().StartsWith("image/")
                    && speakerVM.ImageFile.Length <= SpeakerViewModel.MaxImageSize)
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
