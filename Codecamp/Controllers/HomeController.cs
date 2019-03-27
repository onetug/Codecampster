using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Codecamp.Models;
using Microsoft.AspNetCore.Authorization;
using Codecamp.Data;
using Microsoft.AspNetCore.Identity;
using Codecamp.BusinessLogic;
using Codecamp.ViewModels;

namespace Codecamp.Controllers
{
    public class HomePageViewModel
    {
        public Event Event { get; set; }
        public List<AnnouncementViewModel> Announcements { get; set; }
        public SponsorViewModel FeaturedSponsor { get;set; }
    }

    public class HomeController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly IEventBusinessLogic _eventBL;
        private readonly IAnnouncementBusinessLogic _announcementBL;
        private readonly ISponsorBusinessLogic _sponsorBL;


        public HomeController(
            CodecampDbContext context,
            UserManager<CodecampUser> userManager,
            IEventBusinessLogic eventBL,
            IAnnouncementBusinessLogic announcementBL,
            ISponsorBusinessLogic sponsorBL)
        {
            _context = context;
            _userManager = userManager;
            _eventBL = eventBL;
            _announcementBL = announcementBL;
            _sponsorBL = sponsorBL;
        }
        [ResponseCache(Duration = 300, Location = ResponseCacheLocation.Any, NoStore = false)]
        public async Task<IActionResult> Index()
        {
            var viewModel = new HomePageViewModel
            {
                Event = await _eventBL.GetActiveEvent(),
                Announcements = await _announcementBL.GetActiveAnnouncementsViewModelForActiveEvent(),
                FeaturedSponsor = await _sponsorBL.GetRandomSponsor()
            };

            return View(viewModel);
        }

        [Authorize]
        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Event()
        {
            ViewData["Message"] = "This page contains the event essentials: date, time, and location";

            return View();
        }

        public IActionResult Location()
        {
            return View();
        }
        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Faq()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
