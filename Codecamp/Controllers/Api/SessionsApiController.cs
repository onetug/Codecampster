using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Codecamp.Controllers.Api
{
    [Route("api/sessions")]
    [ApiExplorerSettings(GroupName = "Sessions")]
    public class SessionsApiController : ControllerBase
    {
        public SessionsApiController(ISessionsApiBusinessLogic logic)
        {
            BusinessLogic = logic;
        }

        private ISessionsApiBusinessLogic BusinessLogic { get; }

        [HttpGet]
        [Produces("application/json", Type = typeof(List<ApiSession>))]
        public IActionResult GetSessionList(int? eventId = null, 
            int? trackId = null,
            int? timeslotId = null,
            bool includeDescriptions = false)
        {
            var apiSessionList = BusinessLogic.GetSessionsList(eventId,
                trackId, timeslotId, includeDescriptions);
            var jsonSessionsList = new JsonResult(apiSessionList);

            return jsonSessionsList;
        }

        [HttpGet("{sessionId}")]
        [Produces("application/json", Type = typeof(ApiSession))]
        public IActionResult GetSession(int sessionId,
            bool includeDescription = false)
        {
            var apiSession = BusinessLogic.GetSession(sessionId, includeDescription);

            if (apiSession == null)
                return NotFound();

            var jsonSession = new JsonResult(apiSession);

            return jsonSession;
        }
    }
}