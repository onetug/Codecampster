using Codecamp.Data;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface IScheduleBusinessLogic
    {
        Task<List<Schedule>> GetSchedule(int eventId);
        Task<List<Schedule>> GetActiveSchedule();
        Task<List<ScheduleViewModel>> GetScheduleViewModel(int eventId);
        Task<List<ScheduleViewModel>> GetActiveScheduleViewModel();
        List<ScheduleViewModel> ToScheduleViewModel(List<Schedule> schedule);
        Task<List<ScheduleViewModel>> GetScheduleViewModelToBuildSchedule(int eventId);
        Task<List<ScheduleViewModel>> GetScheduleViewModelToBuildScheduleForActiveEvent();
        Task<List<Track>> GetAvailableTracks(int eventId);
        Task<List<Timeslot>> GetAvailableTimeslots(int eventId, int trackId);
    }

    public class ScheduleBusinessLogic : IScheduleBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public ScheduleBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get the schedule for the specified eventId
        /// </summary>
        /// <param name="eventId">The desired eventId</param>
        /// <returns>The collection of Schedule objects</returns>
        public async Task<List<Schedule>> GetSchedule(int eventId)
        {
            return await _context.CodecampSchedule
                .Include(s => s.Session)
                .Include(s => s.Timeslot)
                .Include(s => s.Track)
                .Where(s => 
                    s.Session.EventId == eventId 
                    && s.Track.EventId == eventId 
                    && s.Timeslot.EventId == eventId)
                .OrderBy(s => s.Track.Name)
                .ThenBy(s => s.Timeslot.StartTime)
                .ToListAsync();
        }

        /// <summary>
        /// Get the schedule for the active event.  If not active
        /// event is specified, an empty list is returned
        /// </summary>
        /// <returns>The collection of Schedule objects</returns>
        public async Task<List<Schedule>> GetActiveSchedule()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return new List<Schedule>();
            else
                return await GetSchedule(activeEvent.EventId);
        }

        public async Task<List<ScheduleViewModel>> GetScheduleViewModel(int eventId)
        {
            var schedule = await GetSchedule(eventId);

            return ToScheduleViewModel(schedule);
        }

        public async Task<List<ScheduleViewModel>> GetActiveScheduleViewModel()
        {
            var schedule = await GetActiveSchedule();

            return ToScheduleViewModel(schedule);
        }

        public List<ScheduleViewModel> ToScheduleViewModel(List<Schedule> schedule)
        {
            var scheduleViewModel
                = from _schedule in schedule
                  // Get session information
                  join session in _context.Sessions on _schedule.SessionId equals session.SessionId
                  // Get event information
                  join _event in _context.Events on _schedule.Session.EventId equals _event.EventId
                  // Get track information
                  join track in _context.Tracks on _schedule.TrackId 
                        equals track.TrackId
                  // Get timeslot information
                  join timeslot in _context.Timeslots on _schedule.TimeslotId 
                        equals timeslot.TimeslotId
                  // Get speakerSession information
                  join _speakerSession in _context.SpeakerSessions 
                        on _schedule.SessionId equals _speakerSession.SessionId 
                        into speakerSessionsGroupJoin
                  select new ScheduleViewModel
                  {
                      SessionId = _schedule.SessionId,
                      Name = session.Name,
                      Description = session.Description,
                      SkillLevelId = session.SkillLevel,
                      SkillLevel = SkillLevel.GetSkillLevelDescription(session.SkillLevel),
                      Keywords = session.Keywords,
                      IsApproved = session.IsApproved,
                      SpeakerSessions = speakerSessionsGroupJoin.ToList(),
                      TrackId = _schedule.TrackId,
                      TrackName = track.Name,
                      RoomNumber = track.RoomNumber,
                      TimeslotId = _schedule.TimeslotId,
                      StartTime = timeslot.StartTime,
                      EndTime = timeslot.EndTime
                  };

            return scheduleViewModel.ToList();
        }

        public async Task<List<ScheduleViewModel>> GetScheduleViewModelToBuildSchedule(int eventId)
        {
            var scheduleToBuild
                // Get the sessions
                = from session in _context.Sessions
                  // Speaker/session information
                  join _speakerSession in _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser) 
                    on session.SessionId equals _speakerSession.SessionId 
                    into speakerSessionLeftJoin
                  // Get schedule info if it exists
                  join _schedule in _context.CodecampSchedule 
                    on session.SessionId equals _schedule.SessionId 
                    into scheduleLeftJoin
                  from schedule in scheduleLeftJoin.DefaultIfEmpty()
                  // Get track information if it exists
                  join _track in _context.Tracks
                    on schedule.TrackId equals _track.TrackId 
                    into trackLeftJoin
                  // Get timeslot inforation if it exists
                  join _timeslot in _context.Timeslots 
                    on schedule.TimeslotId equals _timeslot.TimeslotId 
                    into timeslotLeftJoin
                  from track in trackLeftJoin.DefaultIfEmpty()
                  from timeslot in timeslotLeftJoin.DefaultIfEmpty()
                  where session.EventId == eventId
                  select new ScheduleViewModel
                  {
                      SessionId = session.SessionId,
                      Name = session.Name,
                      Description = session.Description,
                      SkillLevelId = session.SkillLevel,
                      SkillLevel = SkillLevel.GetSkillLevelDescription(session.SkillLevel),
                      Keywords = session.Keywords,
                      IsApproved = session.IsApproved,
                      SpeakerSessions = speakerSessionLeftJoin.ToList(),
                      TrackId = track != null ? track.TrackId : (int?)null,
                      TrackName = track != null ? track.Name : string.Empty,
                      RoomNumber = track != null ? track.RoomNumber : string.Empty,
                      TimeslotId = timeslot != null ? timeslot.TimeslotId : (int?)null,
                      StartTime = timeslot != null ? timeslot.StartTime : (DateTime?)null,
                      EndTime = timeslot != null ? timeslot.EndTime : (DateTime?)null
                  };

            scheduleToBuild = scheduleToBuild
                .OrderBy(s => s.TrackName)
                .ThenBy(s => s.StartTime);

            return await scheduleToBuild.ToListAsync();
        }

        public async Task<List<ScheduleViewModel>> GetScheduleViewModelToBuildScheduleForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return new List<ScheduleViewModel>();
            else
                return await GetScheduleViewModelToBuildSchedule(activeEvent.EventId);
        }

        public async Task<List<Track>> GetAvailableTracks(int eventId)
        {
            var availableTracks = new List<Track>();

            // Get the count of the number of timeslots
            var timeslotCount = await _context.Timeslots.Where(t => t.EventId == eventId).CountAsync();

            foreach (var track in _context.Tracks.Where(t => t.EventId == eventId))
            {
                // The assigned timeslots
                var assignedTimeslots = from item in _context.CodecampSchedule
                                        where item.TrackId == track.TrackId
                                        select item.TimeslotId;

                // If all the timeslots are NOT assigned (i.e., the count of 
                // assigned timeslots is less than the number of timeslots)
                if (await assignedTimeslots.CountAsync() < timeslotCount)
                    // One or more timeslots are NOT assigned, the track is 
                    // still open
                    availableTracks.Add(track);
            }

            availableTracks = availableTracks
                .OrderBy(t => t.Name)
                .ToList();

            return availableTracks;
        }

        public async Task<List<Timeslot>> GetAvailableTimeslots(int eventId, int trackId)
        {
            // The assigned timeslots for the specified track
            var timeslotsAssignedForTrack = from item in _context.CodecampSchedule.Include(s => s.Track)
                                            where item.TrackId == trackId && item.Track.EventId == eventId
                                            select item.TimeslotId;

            // The available timeslots for the specified track
            var availableTimeslotsForTrack = from timeslot in _context.Timeslots
                                             where timeslotsAssignedForTrack.Contains(timeslot.TimeslotId) == false
                                             select timeslot;

            availableTimeslotsForTrack = availableTimeslotsForTrack
                .OrderBy(t => t.StartTime);

            return await availableTimeslotsForTrack.ToListAsync();
        }
    }
}
