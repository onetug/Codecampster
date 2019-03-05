using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codecamp.BusinessLogic;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codecamp.Controllers.Api
{
    [Route("api/schedule")]
    [ApiExplorerSettings(GroupName = "Sechedule")]
    [ApiController]
    public class ScheduleApiController : ControllerBase
    {
        private IScheduleBusinessLogic _scheduleBL { get; set; }
        private ISessionBusinessLogic _sessionBL { get; set; }

        public ScheduleApiController(IScheduleBusinessLogic scheduleBL,
            ISessionBusinessLogic sessionBL)
        {
            _scheduleBL = scheduleBL;
            _sessionBL = sessionBL;
        }

        /// <summary>
        /// Gets the avaialble TrackViewModel objects for the active event
        /// </summary>
        /// <returns>Collection of TrackViewModel objects</returns>
        [HttpGet("availableTracks")]
        [Produces("application/json")]
        public async Task<ActionResult<List<TrackViewModel>>> GetAvailableTracks()
        {
            var availableTracks
                = await _scheduleBL.GetAvailableTrackViewModels();

            return availableTracks;
        }

        /// <summary>
        /// Gets the available TimeslotViewModel objects for the avtive event
        /// </summary>
        /// <param name="trackId">The desired TrackId</param>
        /// <returns>The collection of TimeslotViewModel objects</returns>
        [HttpGet("availableTimeslots/{trackId}")]
        [Produces("application/json")]
        public async Task<ActionResult<List<TimeslotViewModel>>> GetAvailableTimeslots(int trackId)
        {
            var availableTimeslots
                = await _scheduleBL.GetAvailableTimeslotViewModels(trackId);

            return availableTimeslots;
        }

        [HttpPost("sessionApproval/{id}")]
        public async Task<ActionResult<Session>> SetApprovalStatus(int id, [FromBody] bool approvalStatus)
        {
            var session = await _sessionBL.GetSession(id);

            session.IsApproved = approvalStatus;

            var result = await _sessionBL.UpdateSession(session);

            if (result == true)
                return StatusCode(StatusCodes.Status200OK);
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}