using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers
{
    [Route("api/Sponsors")]
    [ApiController]
    public class SponsorsApiController : ControllerBase
    {
        private readonly CodecampDbContext _context;
        private readonly ISponsorBusinessLogic _sponsorBL;

        public SponsorsApiController(
            CodecampDbContext context,
            ISponsorBusinessLogic sponsorBL)
        {
            _context = context;
            _sponsorBL = sponsorBL;
        }

        [Produces("application/json")]
        [HttpGet("image/{sponsorId}")]
        public async Task<IActionResult> GetSponsorImage(int sponsorId)
        {
            if (!await _sponsorBL.SponsorExists(sponsorId))
                return NotFound();

            var sponsor = await _context.Sponsors.FindAsync(sponsorId);
            if (sponsor == null)
                return NotFound();

            if (sponsor.Image.Length > 0)
            {
                return new JsonResult(new { imageSrc = string.Format(
                    "data:image;base64,{0}",
                    Convert.ToBase64String(sponsor.Image))});
            }
            else
            {
                return new JsonResult(new { imageSrc = "/images/default_user_icon.jpg" });
            }
        }
    }
}