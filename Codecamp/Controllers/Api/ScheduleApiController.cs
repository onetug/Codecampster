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
    [ApiExplorerSettings(GroupName = "Schedule")]
    [ApiController]
    public class ScheduleApiController : ControllerBase
    {
        private IScheduleBusinessLogic _scheduleBL { get; set; }
        private ISessionBusinessLogic _sessionBL { get; set; }

        public ScheduleApiController(
            IScheduleBusinessLogic scheduleBL,
            ISessionBusinessLogic sessionBL)
        {
            _scheduleBL = scheduleBL;
            _sessionBL = sessionBL;
        }

        ///// <summary>
        ///// Gets the avaialble TrackViewModel objects for the active event
        ///// </summary>
        ///// <returns>Collection of TrackViewModel objects</returns>
        //[HttpGet("availableTracks/{sessionId}")]
        //[Produces("application/json", Type = typeof(List<TrackViewModel>))]
        //public async Task<ActionResult<List<TrackViewModel>>> GetAvailableTracks(int sessionId)
        //{
        //    var availableTracks
        //        = await _scheduleBL.GetAvailableTrackViewModels(sessionId);

        //    return availableTracks;
        //}

        /// <summary>
        /// Gets the available tracks for assignment to sessions.  The collection
        /// is returned as a dictionary with session id as the key
        /// </summary>
        /// <returns>Dictionary of TrackViewModels for each session</returns>
        [HttpGet("allAvailableTracks")]
        [Produces("application/json", Type = typeof(Dictionary<int, List<TrackViewModel>>))]
        public async Task<ActionResult<Dictionary<int, List<TrackViewModel>>>> GetAvailableTrackViewModelsForSessions()
        {
            var availableTracks
                = await _scheduleBL.GetAvailableTrackViewModelsForSessions();

            return availableTracks;
        }

        /// <summary>
        /// Gets the available timeslots for assignment to sessions.  The collection
        /// is returned as a dictionary with session id as the key
        /// </summary>
        /// <returns>Dictionary of TimeslotViewModels for each session</returns>
        [HttpGet("allAvailableTimeslots")]
        [Produces("application/json", Type = typeof(Dictionary<int, List<TimeslotViewModel>>))]
        public async Task<ActionResult<Dictionary<int, List<TimeslotViewModel>>>> GetAvailableTimeslotViewModelsForSessions()
        {
            var availableTimeslots
                = await _scheduleBL.GetAvailableTimeslotViewModelsForSessions();

            return availableTimeslots;
        }

        ///// <summary>
        ///// Gets the available TimeslotViewModel objects for the avtive event
        ///// </summary>
        ///// <param name="trackId">The desired TrackId</param>
        ///// <returns>The collection of TimeslotViewModel objects</returns>
        //[HttpGet("availableTimeslots/{sessionId}")]
        //[Produces("application/json")]
        //public async Task<ActionResult<List<TimeslotViewModel>>> GetAvailableTimeslots(int sessionId)
        //{
        //    var availableTimeslots
        //        = await _scheduleBL.GetAvailableTimeslotViewModels(sessionId);

        //    return availableTimeslots;
        //}

        [HttpPost("sessionApproval/{sessionId}")]
        [Produces("application/json", Type = typeof(bool))]
        public async Task<ActionResult<bool>> SetApprovalStatus(int sessionId, [FromBody] bool approvalStatus)
        {
            var session = await _sessionBL.GetSession(sessionId);

            var originalValue = session.IsApproved;

            session.IsApproved = approvalStatus;

            var result = await _sessionBL.UpdateSession(session);

            if (result == true)
                return Ok(session.IsApproved);
            else
                return StatusCode(StatusCodes.Status400BadRequest, originalValue);
        }

        [HttpPost("assignTrackToSession/{sessionId}")]
        [Produces("application/json", Type = typeof(int?))]
        public async Task<ActionResult<int>> AssignTrackToSession(int sessionId,
            [FromBody] int trackId)
        {
            var session = await _sessionBL.GetSession(sessionId);

            var originalValue = session.TrackId;

            session.TrackId = trackId == 0 ? (int?)null : trackId;

            // The track changed, reset the timeslot also
            session.TimeslotId = (int?)null;

            var result = await _sessionBL.UpdateSession(session);

            if (result == true)
                return Ok(session.TrackId);
            else
                return StatusCode(StatusCodes.Status400BadRequest, originalValue);
        }

        [HttpPost("assignTimeslotToSession/{sessionId}")]
        [Produces("application/json", Type = typeof(int?))]
        public async Task<ActionResult<int>> AssignTimeslotToSession(int sessionId,
            [FromBody] int timeslotId)
        {
            var session = await _sessionBL.GetSession(sessionId);

            var originalValue = session.TimeslotId;

            session.TimeslotId = timeslotId == 0 ? (int?)null : timeslotId;

            var result = await _sessionBL.UpdateSession(session);

            if (result == true)
                return Ok(session.TimeslotId);
            else
                return StatusCode(StatusCodes.Status400BadRequest, originalValue);
        }
    }
}