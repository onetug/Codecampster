using Codecamp.BusinessLogic;
using Codecamp.BusinessLogic.Api;
using Codecamp.Models.Api;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codecamp.Controllers.Api
{
    [Route("api/sessions")]
    [ApiExplorerSettings(GroupName = "Sessions")]
    [ApiController]
    public class SessionsApiController : ControllerBase
    {
        private ISessionsApiBusinessLogic BusinessLogic { get; }
        private ISessionBusinessLogic _sessionBL { get; set; }

        public SessionsApiController(
            ISessionsApiBusinessLogic logic,
            ISessionBusinessLogic sessionBL)
        {
            BusinessLogic = logic;
            _sessionBL = sessionBL;
        }

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

        [HttpPost("toggleFavoriteSession/{sessionId}")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<ActionResult<bool>> ToggleFavoriteSession(int sessionId, [FromBody] string userId)
        {
            var result = await _sessionBL.ToggleFavoriteSession(sessionId, userId);

            if (result == true)
                return Ok(result);
            else
                return StatusCode(StatusCodes.Status400BadRequest, result);
        }
    }
}