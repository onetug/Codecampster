using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/events")]
    [ApiExplorerSettings(GroupName = "Events")]
    public class EventsApiController : ControllerBase
    {
        public EventsApiController(IEventsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private IEventsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiEvent>))]
        public IActionResult GetEventsList()
        {
            var apiEventsList = BusinessLogic.GetEventsList();
            var jsonEventsList = new JsonResult(apiEventsList);

            return jsonEventsList;
        }

        [HttpGet("{eventId}")]
        [Produces("application/json", Type = typeof(ApiEvent))]
        public IActionResult GetEvent(int eventId)
        {
            var apiEvent = BusinessLogic.GetEvent(eventId);

            if (apiEvent == null)
                return NotFound();

            var jsonEvent = new JsonResult(apiEvent);

            return jsonEvent;
        }
    }
}