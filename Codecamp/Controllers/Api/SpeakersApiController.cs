using Codecamp.BusinessLogic.Api;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Codecamp.Controllers.Api
{
    [Route("api/speakers")]
    public class SpeakersApiController : ControllerBase //: ApiController
    {
        public SpeakersApiController(ISpeakersApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ISpeakersApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetSpeakersList()
        {
            var apiSpeakersList = await BusinessLogic.GetSpeakersList();
            var jsonSpeakersList = new JsonResult(apiSpeakersList);

            return jsonSpeakersList;
        }

        [HttpGet("{speakerId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetSpeaker(int speakerId,
            bool includeDetails = false)
        {
            var apiSpeaker = await BusinessLogic.GetApiSpeaker(speakerId, includeDetails);

            if (apiSpeaker == null)
                return NotFound();

            var jsonSpeaker = new JsonResult(apiSpeaker);

            return jsonSpeaker;
        }

        // TODO Support JPEG?
        [HttpGet("{speakerId}/image")]
        [Produces("image/png")]
        public async Task<IActionResult> GetSpeakerImage(int speakerId)
        {
            var webSpeaker = await BusinessLogic.GetWebSpeaker(speakerId);

            if (webSpeaker == null)
                // TODO return contents of "/images/default_user_icon.jpg" instead?
                return NotFound();

            if (webSpeaker.Image == null)
                return File("/images/default_user_icon.jpg", "image/png");

            return File(webSpeaker.Image, "image/png");
        }
    }
}