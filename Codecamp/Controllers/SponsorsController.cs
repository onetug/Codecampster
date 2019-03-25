using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers
{
    public class SponsorsController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly ISponsorBusinessLogic _sponsorBL;
        private readonly IEventBusinessLogic _eventBL;

        private SponsorViewModel Sponsor { get; set; }

        public SponsorsController(
            CodecampDbContext context,
            ISponsorBusinessLogic sponsorBL,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _sponsorBL = sponsorBL;
            _eventBL = eventBL;
        }

        // GET: Sponsors
        public async Task<IActionResult> Index()
        {
            var sponsorsVM 
                = await _sponsorBL
                .GetSponsorsViewModelForActiveEvent();

            return View(sponsorsVM);
        }

        // GET: Sponsors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            // If no id is specified, return not found
            if (id.HasValue == false)
                return NotFound();

            // If the sponsor does not exist, return not found
            if (!await _sponsorBL.SponsorExists(id.Value))
                return NotFound();

            // Find the specified sponsor
            var sponsorVM = await _sponsorBL.GetSponsorViewModel(id.Value);

            // If the sponsor is nnot found, return not found
            if (sponsorVM == null)
                return NotFound();

            // Else return the view with the sponsor information
            return View(sponsorVM);
        }

        [Authorize]
        // GET: Sponsors/Create
        public IActionResult Create()
        {
            ViewBag.SponsorshipLevels = SponsorLevel.GetSponsorshipLevels();
            return View();
        }

        // POST: Sponsors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("SponsorId,CompanyName,SponsorLevel,Bio,TwitterHandle,WebsiteUrl,ImageFile,PointOfContact,EmailAddress,PhoneNumber")] SponsorViewModel sponsorVM)
        {
            if (ModelState.IsValid)
            {
                var theEvent = await _eventBL.GetActiveEvent();
                if (theEvent != null)
                {
                    var sponsor = new Sponsor
                    {
                        SponsorId = sponsorVM.SponsorId,
                        CompanyName = sponsorVM.CompanyName,
                        SponsorLevel = sponsorVM.SponsorLevel,
                        Bio = sponsorVM.Bio,
                        TwitterHandle = sponsorVM.TwitterHandle,
                        WebsiteUrl = sponsorVM.WebsiteUrl,
                        PointOfContact = sponsorVM.PointOfContact,
                        EmailAddress = sponsorVM.EmailAddress,
                        PhoneNumber = sponsorVM.PhoneNumber,
                        EventId = theEvent.EventId
                    };

                    // Convert the image to a byte array, reduce the size to 500px x 500px
                    // and store it in the database
                    if (sponsorVM.ImageFile != null &&
                        sponsorVM.ImageFile.ContentType.ToLower().StartsWith("image/")
                        && sponsorVM.ImageFile.Length <= SponsorViewModel.MaxImageSize)
                    {
                        MemoryStream ms = new MemoryStream();
                        sponsorVM.ImageFile.OpenReadStream().CopyTo(ms);

                        sponsor.Image
                            = _sponsorBL.ResizeImage(ms.ToArray());
                    }

                    await _sponsorBL.CreateSponsor(sponsor);

                    return RedirectToAction(nameof(Index));
                }
            }

            ViewBag.SponsorshipLevels = SponsorLevel.GetSponsorshipLevels();
            return View(sponsorVM);
        }

        // GET: Sponsors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            if (!await _sponsorBL.SponsorExists(id.Value))
                return NotFound();

            var sponsorVM = await _sponsorBL.GetSponsorViewModel(id.Value);
            if (sponsorVM == null)
                return NotFound();

            ViewBag.SponsorshipLevels = SponsorLevel.GetSponsorshipLevels();
            return View(sponsorVM);
        }

        // POST: Sponsors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(int id, 
            [Bind("SponsorId,CompanyName,SponsorLevel,Bio,TwitterHandle,WebsiteUrl,ImageFile,PointOfContact,EmailAddress,PhoneNumber")] SponsorViewModel sponsorVM)
        {
            if (ModelState.IsValid)
            {
                if (id != sponsorVM.SponsorId)
                    return NotFound();

                var sponsor = await _sponsorBL.GetSponsor(sponsorVM.SponsorId);

                sponsor.CompanyName = sponsorVM.CompanyName;
                sponsor.SponsorLevel = sponsorVM.SponsorLevel;
                sponsor.Bio = sponsorVM.Bio;
                sponsor.TwitterHandle = sponsorVM.TwitterHandle;
                sponsor.WebsiteUrl = sponsorVM.WebsiteUrl;
                sponsor.PointOfContact = sponsorVM.PointOfContact;
                sponsor.EmailAddress = sponsorVM.EmailAddress;
                sponsor.PhoneNumber = sponsorVM.PhoneNumber;

                // Convert the image to a byte array, reduce the size to 500px x 500px
                // and store it in the database
                if (sponsorVM.ImageFile != null &&
                    sponsorVM.ImageFile.ContentType.ToLower().StartsWith("image/")
                    && sponsorVM.ImageFile.Length <= SponsorViewModel.MaxImageSize)
                {
                    MemoryStream ms = new MemoryStream();
                    sponsorVM.ImageFile.OpenReadStream().CopyTo(ms);

                    sponsor.Image
                        = _sponsorBL.ResizeImage(ms.ToArray());
                }

                var result = await _sponsorBL.UpdateSponsor(sponsor);

                if (result == false)
                    return NotFound();
                else
                    return RedirectToAction(nameof(Index));
            }

            ViewBag.SponsorshipLevels = SponsorLevel.GetSponsorshipLevels();
            return View(sponsorVM);
        }

        // GET: Sponsors/Delete/5
        [Authorize(Policy = "RequireAdminRole")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (!id.HasValue)
                return NotFound();

            if (!await _sponsorBL.SponsorExists(id.Value))
                return NotFound();

            var sponsorVM = await _sponsorBL.GetSponsorViewModel(id.Value);

            if (sponsorVM == null)
                return NotFound();

            return View(sponsorVM);
        }

        // POST: Sponsors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "RequireAdminRole")]
        public async Task <IActionResult> DeleteConfirmed(int id)
        {
            await _sponsorBL.DeleteSponsor(id);

            return RedirectToAction(nameof(Index));
        }
    }
}