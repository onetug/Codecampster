using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/sponsors")]
    [ApiExplorerSettings(GroupName = "Sponsors")]
    public class SponsorsApiController : ControllerBase
    {
        public SponsorsApiController(ISponsorsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ISponsorsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiSponsor>))]
        public IActionResult GetSponsorList(int? eventId = null)
        {
            var apiSponsorList = BusinessLogic.GetSponsorsList(eventId);
            var jsonSponsorsList = new JsonResult(apiSponsorList);

            return jsonSponsorsList;
        }

        [HttpGet("levels")]
        [Produces("application/json", Type = typeof(List<SponsorLevel>))]
        public IActionResult GetSponsorLevels(int? eventId = null)
        {
            var sponsorLevels = BusinessLogic.GetSponsorLevels();
            var jsonSponsorsList = new JsonResult(sponsorLevels);

            return jsonSponsorsList;
        }

        // TODO Support JPEG?
        [HttpGet("{sponsorId}/image")]
        [Produces("image/png", Type = typeof(FileResult))]
        public IActionResult GetSponsorImage(int sponsorId)
        {
            var webSponsor = BusinessLogic.GetWebSponsor(sponsorId);

            if (webSponsor == null)
                // TODO return contents of "/images/default_user_icon.jpg" instead?
                return NotFound();

            // TODO Need a better default for missing sponsor images
            if (webSponsor.Image == null)
                return File("/images/default_user_icon.jpg", "image/png");

            return File(webSponsor.Image, "image/png");
        }
    }
}
