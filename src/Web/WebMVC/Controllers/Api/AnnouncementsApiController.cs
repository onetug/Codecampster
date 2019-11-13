using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/announcements")]
    [ApiExplorerSettings(GroupName = "Announcements")]
    public class AnnouncementsApiController : ControllerBase
    {
        public AnnouncementsApiController(IAnnouncementsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private IAnnouncementsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiAnnouncement>))]
        public IActionResult GetAnnouncementList(int? eventId = null)
        {
            var apiAnnouncementList = BusinessLogic.GetAnnouncementsList(eventId);
            var jsonAnnouncementsList = new JsonResult(apiAnnouncementList);

            return jsonAnnouncementsList;
        }
    }
}
