using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/tracks")]
    [ApiExplorerSettings(GroupName = "Tracks")]
    public class TracksApiController : ControllerBase
    {
        public TracksApiController(ITracksApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ITracksApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiTrack>))]
        public IActionResult GetTrackList(int? eventId = null)
        {
            var apiTrackList = BusinessLogic.GetTracksList(eventId);
            var jsonTracksList = new JsonResult(apiTrackList);

            return jsonTracksList;
        }
    }
}
