using System;
using System.Collections.Generic;
using System.IO;
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

        [Produces("application/octet-stream")]
        [HttpGet("image/{sponsorId}")]
        public async Task<IActionResult> GetSponsorImage(int sponsorId)
        {
            if (!await _sponsorBL.SponsorExists(sponsorId))
                return NotFound();

            var sponsor = await _context.Sponsors.FindAsync(sponsorId);
            if (sponsor == null || sponsor.Image == null || sponsor.Image.Length == 0)
                return NotFound();

            return File(new MemoryStream(sponsor.Image), "application/octet-stream");
        }
    }
}