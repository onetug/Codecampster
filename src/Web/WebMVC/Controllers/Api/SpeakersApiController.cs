using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/speakers")]
    [ApiExplorerSettings(GroupName = "Speakers")]
    public class SpeakersApiController : ControllerBase
    {
        public SpeakersApiController(ISpeakersApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ISpeakersApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiSpeaker>))]
        public IActionResult GetSpeakersList(int? eventId = null,
            bool includeDetails = false)
        {
            var apiSpeakersList = BusinessLogic.GetSpeakersList(eventId, includeDetails);

            var jsonSpeakersList = new JsonResult(apiSpeakersList);

            return jsonSpeakersList;
        }

        [HttpGet("{speakerId}")]
        [Produces("application/json", Type = typeof(ApiSpeaker))]
        public IActionResult GetSpeaker(int speakerId,
            bool includeDetails = false)
        {
            var apiSpeaker = BusinessLogic.GetApiSpeaker(speakerId, includeDetails);

            if (apiSpeaker == null)
                return NotFound();

            var jsonSpeaker = new JsonResult(apiSpeaker);

            return jsonSpeaker;
        }

        // TODO Support JPEG?
        [HttpGet("{speakerId}/image")]
        [Produces("image/png", Type = typeof(FileResult))]
        public IActionResult GetSpeakerImage(int speakerId)
        {
            var webSpeaker = BusinessLogic.GetWebSpeaker(speakerId);

            if (webSpeaker == null)
                // TODO return contents of "/images/default_user_icon.jpg" instead?
                return NotFound();

            if (webSpeaker.Image == null)
                return File("/images/default_user_icon.jpg", "image/png");

            return File(webSpeaker.Image, "image/png");
        }
    }
}