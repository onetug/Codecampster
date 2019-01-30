using Codecamp.BusinessLogic.Api;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Codecamp.Controllers.Api
{
    [Route("api/events")]
    public class EventsApiController : ControllerBase //: ApiController
    {
        public EventsApiController(IEventsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private IEventsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetEventsList()
        {
            var apiEventsList = await BusinessLogic.GetEventsList();
            var jsonEventsList = new JsonResult(apiEventsList);

            return jsonEventsList;
        }

        [HttpGet("{eventId}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetEvent(int eventId)
        {
            var apiEvent = await BusinessLogic.GetEvent(eventId);

            if (apiEvent == null)
                return NotFound();

            var jsonEvent = new JsonResult(apiEvent);

            return jsonEvent;
        }
    }
}