using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Data;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers
{
    [Route("api/Speakers")]
    [ApiController]
    public class SpeakersApiController : ControllerBase
    {
        private readonly CodecampDbContext _context;
        private readonly ISpeakerBusinessLogic _speakerBL;

        public SpeakersApiController(
            CodecampDbContext context,
            ISpeakerBusinessLogic speakerBL)
        {
            _context = context;
            _speakerBL = speakerBL;
        }

        [Produces("application/json")]
        [HttpGet("image/{speakerId}")]
        public async Task<IActionResult> GetSpeakerImage(int speakerId)
        {
            if (!await _speakerBL.SpeakerExists(speakerId))
                return NotFound();

            var speaker = await _context.Speakers.FindAsync(speakerId);
            if (speaker == null)
                return NotFound();

            if (speaker.Image.Length > 0)
            {
                return new JsonResult(new { imageSrc = String.Format("data:image;base64,{0}",
                    Convert.ToBase64String(speaker.Image)) });
            }
            else
            {
                return new JsonResult(new { imageSrc = "/images/default_user_icon.jpg" });
            }
        }
    }
}