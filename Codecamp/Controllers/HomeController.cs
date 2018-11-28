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

namespace Codecamp.Controllers
{
    public class HomeController : Controller
    {
        private readonly CodecampDbContext _context;
        private readonly UserManager<CodecampUser> _userManager;
        private readonly IEventBusinessLogic _eventBL;

        public HomeController(
            CodecampDbContext context,
            UserManager<CodecampUser> userManager,
            IEventBusinessLogic eventBL)
        {
            _context = context;
            _userManager = userManager;
            _eventBL = eventBL;
        }

        public async Task<IActionResult> Index()
        {
            var theEvent = await _eventBL.GetActiveEvent();

            return View(theEvent);
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

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Privacy()
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
