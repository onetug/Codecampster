using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/timeslots")]
    [ApiExplorerSettings(GroupName = "Timeslots")]
    public class TimeslotsApiController : ControllerBase
    {
        public TimeslotsApiController(ITimeslotsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ITimeslotsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiTimeslot>))]
        public IActionResult GetTimeslotList(int? eventId = null)
        {
            var apiTimeslotList = BusinessLogic.GetTimeslotsList(eventId);
            var jsonTimeslotsList = new JsonResult(apiTimeslotList);

            return jsonTimeslotsList;
        }
    }
}
