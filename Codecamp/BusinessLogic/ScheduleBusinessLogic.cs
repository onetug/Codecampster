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
        Task<List<ScheduleViewModel>> GetScheduleViewModel(int eventId);
        Task<List<ScheduleViewModel>> GetActiveScheduleViewModel();
        Task<List<Track>> GetAvailableTracks(int sessionId, int? eventId = null);
        Task<List<TrackViewModel>> GetAvailableTrackViewModels(int sessionId, int? eventId = null);
        Task<List<Timeslot>> GetAvailableTimeslots(int sessionId, int? eventId = null);
        Task<List<TimeslotViewModel>> GetAvailableTimeslotViewModels(int sessionId, int? eventId = null);
        Task<Dictionary<int, List<TrackViewModel>>> GetAvailableTrackViewModelsForSessions(int? eventId = null);
        Task<Dictionary<int, List<TimeslotViewModel>>> GetAvailableTimeslotViewModelsForSessions(int? eventId = null);
    }

    public class ScheduleBusinessLogic : IScheduleBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public ScheduleBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<ScheduleViewModel>> GetScheduleViewModel(int eventId)
        {
            var tempSchedule
                // Get the sessions
                = from session in _context.Sessions
                      // Speaker/session information
                  join speakerSession in _context.SpeakerSessions on session.SessionId equals speakerSession.SessionId
                  join speaker in _context.Speakers on speakerSession.SpeakerId equals speaker.SpeakerId
                  join codecampUser in _context.CodecampUsers on speaker.CodecampUserId equals codecampUser.Id
                  // Get track information if it exists
                  join _track in _context.Tracks
                    on session.TrackId equals _track.TrackId 
                    into trackLeftJoin
                  // Get timeslot inforation if it exists
                  join _timeslot in _context.Timeslots 
                    on session.TimeslotId equals _timeslot.TimeslotId 
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
                      Speakers = codecampUser.FullName,
                      TrackId = track != null ? track.TrackId : (int?)null,
                      TrackName = track != null ? track.Name : string.Empty,
                      RoomNumber = track != null ? track.RoomNumber : string.Empty,
                      TimeslotId = timeslot != null ? timeslot.TimeslotId : (int?)null,
                      StartTime = timeslot != null ? timeslot.StartTime : DateTime.MinValue,
                      EndTime = timeslot != null ? timeslot.EndTime : DateTime.MinValue
                  };

            var schedule = from _schedule in tempSchedule
                           group _schedule by new
                           {
                               _schedule.SessionId,
                               _schedule.Name,
                               _schedule.Description,
                               _schedule.SkillLevelId,
                               _schedule.SkillLevel,
                               _schedule.Keywords,
                               _schedule.IsApproved,
                               _schedule.TrackId,
                               _schedule.TrackName,
                               _schedule.RoomNumber,
                               _schedule.TimeslotId,
                               _schedule.StartTime,
                               _schedule.EndTime
                           } into groupSchedule
                           select new ScheduleViewModel
                           {
                               SessionId = groupSchedule.Key.SessionId,
                               Name = groupSchedule.Key.Name,
                               Description = groupSchedule.Key.Description,
                               SkillLevelId = groupSchedule.Key.SkillLevelId,
                               SkillLevel = groupSchedule.Key.SkillLevel,
                               Keywords = groupSchedule.Key.Keywords,
                               IsApproved = groupSchedule.Key.IsApproved,
                               TrackId = groupSchedule.Key.TrackId,
                               TrackName = groupSchedule.Key.TrackName,
                               RoomNumber = groupSchedule.Key.RoomNumber,
                               TimeslotId = groupSchedule.Key.TimeslotId,
                               StartTime = groupSchedule.Key.StartTime,
                               EndTime = groupSchedule.Key.EndTime,
                               Speakers = string.Join(",", groupSchedule.Select(gs => gs.Speakers))
                           };

            schedule = schedule
                .OrderByDescending(s => s.IsApproved)
                .ThenBy(s => s.TrackName)
                .ThenBy(s => s.StartTime.Value.TimeOfDay);

            return await schedule.ToListAsync();
        }

        public async Task<List<ScheduleViewModel>> GetActiveScheduleViewModel()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return new List<ScheduleViewModel>();
            else
                return await GetScheduleViewModel(activeEvent.EventId);
        }

        public async Task<List<Track>> GetAvailableTracks(int sessionId, int? eventId = null)
        {
            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            // Get the session
            var session
                = await _context.Sessions
                .Where(s => s.SessionId == sessionId)
                .FirstOrDefaultAsync();

            // Is a track already assigned
            var currentlyAssignedTrackId
                = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

            var availableTracks = new List<Track>();

            // Get the count of the number of timeslots
            var timeslotCount = await _context.Timeslots.Where(t => t.EventId == eventId).CountAsync();

            foreach (var track in _context.Tracks.Where(t => t.EventId == eventId))
            {
                // The assigned timeslots
                var assignedTimeslots = from item in _context.Sessions
                                        where item.TrackId == track.TrackId
                                        select item.TimeslotId;

                // If all the timeslots are NOT assigned (i.e., the count of 
                // assigned timeslots is less than the number of timeslots)
                // or the track is currently assigned to the session
                if (await assignedTimeslots.CountAsync() < timeslotCount
                    || track.TrackId == currentlyAssignedTrackId)
                    // One or more timeslots are NOT assigned, the track is 
                    // still open
                    availableTracks.Add(track);
            }

            availableTracks = availableTracks
                .OrderBy(t => t.Name)
                .ToList();

            return availableTracks;
        }

        public async Task<List<TrackViewModel>> GetAvailableTrackViewModels(
            int sessionId, int? eventId = null)
        {
            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            var session
                = await _context.Sessions
                .Where(s => s.SessionId == sessionId)
                .FirstOrDefaultAsync();

            var currentlyAssignedTrackId
                = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

            var availableTrackViewModels = new List<TrackViewModel>();

            // Get the count of the number of timeslots
            var timeslotCount = await _context.Timeslots.Where(t => t.EventId == eventId).CountAsync();

            var trackViewModels = from track in _context.Tracks.Where(t => t.EventId == eventId)
                                  select new TrackViewModel
                                  {
                                      TrackId = track.TrackId,
                                      DisplayName = string.Format("{0} ({1})", track.Name, track.RoomNumber),
                                      Name = track.Name,
                                      RoomNumber = track.RoomNumber
                                  };

            foreach (var trackViewModel in trackViewModels)
            {
                // The assigned timeslots
                var assignedTimeslots = from item in _context.Sessions
                                        where item.TrackId == trackViewModel.TrackId
                                        select item.TimeslotId;

                // If all the timeslots are NOT assigned (i.e., the count of 
                // assigned timeslots is less than the number of timeslots)
                if (await assignedTimeslots.CountAsync() < timeslotCount
                    || trackViewModel.TrackId == currentlyAssignedTrackId)
                    // One or more timeslots are NOT assigned, the track is 
                    // still open
                    availableTrackViewModels.Add(trackViewModel);
            }

            availableTrackViewModels = availableTrackViewModels
                .OrderBy(t => t.Name)
                .ToList();

            return availableTrackViewModels;
        }

        public async Task<List<Timeslot>> GetAvailableTimeslots(int sessionId, int? eventId = null)
        {
            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            var session
                = await _context.Sessions
                .Where(s => s.SessionId == sessionId)
                .FirstOrDefaultAsync();

            var currentlyAssignedTrackId
                = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

            var currentlyAssignedTimeslotId
                = session != null ? session.TimeslotId.HasValue ? session.TimeslotId.Value : 0 : 0;

            // The assigned timeslots for the specified track
            var timeslotsAssignedForTrack = from item in _context.Sessions.Include(s => s.Track)
                                            where item.TrackId == currentlyAssignedTrackId 
                                            && item.Track.EventId == eventId
                                            select item.TimeslotId;

            // The available timeslots for the specified track
            var availableTimeslotsForTrack = from timeslot in _context.Timeslots
                                             where timeslotsAssignedForTrack.Contains(timeslot.TimeslotId) == false
                                             || timeslot.TimeslotId == currentlyAssignedTimeslotId
                                             select timeslot;

            availableTimeslotsForTrack = availableTimeslotsForTrack
                .OrderBy(t => t.StartTime.TimeOfDay);

            return await availableTimeslotsForTrack.ToListAsync();
        }

        public async Task<List<TimeslotViewModel>> GetAvailableTimeslotViewModels(int sessionId, int? eventId = null)
        {
            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            var session
                = await _context.Sessions
                .Where(s => s.SessionId == sessionId)
                .FirstOrDefaultAsync();

            var currentlyAssignedTrackId
                = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

            var currentlyAssignedTimeslotId
                = session != null ? session.TimeslotId.HasValue ? session.TimeslotId.Value : 0 : 0;

            // The assigned timeslots for the specified track
            var timeslotsAssignedForTrack = from item in _context.Sessions.Include(s => s.Track)
                                            where item.TrackId == currentlyAssignedTrackId && item.Track.EventId == eventId
                                            select item.TimeslotId;

            // The available timeslots for the specified track
            var availableTimeslotViewModelsForTrack = from timeslot in _context.Timeslots
                                                      where timeslotsAssignedForTrack.Contains(timeslot.TimeslotId) == false
                                                      || timeslot.TimeslotId == currentlyAssignedTimeslotId
                                                      select new TimeslotViewModel
                                                      {
                                                          TimeslotId = timeslot.TimeslotId,
                                                          DisplayName = string.Format("{0:HH:mm:ss} - {1:HH:mm:ss}", timeslot.StartTime, timeslot.EndTime),
                                                          StartTime = timeslot.StartTime,
                                                          EndTime = timeslot.EndTime,
                                                          ContainsNoSessions = timeslot.ContainsNoSessions
                                                      };

            availableTimeslotViewModelsForTrack = availableTimeslotViewModelsForTrack
                .OrderBy(t => t.StartTime.TimeOfDay);

            return await availableTimeslotViewModelsForTrack.ToListAsync();
        }

        public async Task<Dictionary<int, List<TrackViewModel>>> GetAvailableTrackViewModelsForSessions(int? eventId = null)
        {
            var results = new Dictionary<int, List<TrackViewModel>>();

            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            foreach (var session in _context.Sessions)
            {
                var currentlyAssignedTrackId
                    = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

                var availableTrackViewModels = new List<TrackViewModel>();

                // Get the count of the number of timeslots
                var timeslotCount = await _context.Timeslots.Where(t => t.EventId == eventId).CountAsync();

                var trackViewModels = from track in _context.Tracks.Where(t => t.EventId == eventId)
                                      select new TrackViewModel
                                      {
                                          TrackId = track.TrackId,
                                          DisplayName = string.Format("{0} ({1})", track.Name, track.RoomNumber),
                                          Name = track.Name,
                                          RoomNumber = track.RoomNumber
                                      };

                foreach (var trackViewModel in trackViewModels)
                {
                    // The assigned timeslots
                    var assignedTimeslots = from item in _context.Sessions
                                            where item.TrackId == trackViewModel.TrackId
                                            select item.TimeslotId;

                    // If all the timeslots are NOT assigned (i.e., the count of 
                    // assigned timeslots is less than the number of timeslots)
                    if (await assignedTimeslots.CountAsync() < timeslotCount
                        || trackViewModel.TrackId == currentlyAssignedTrackId)
                        // One or more timeslots are NOT assigned, the track is 
                        // still open
                        availableTrackViewModels.Add(trackViewModel);
                }

                availableTrackViewModels = availableTrackViewModels
                    .OrderBy(t => t.Name)
                    .ToList();

                results.Add(session.SessionId, availableTrackViewModels);
            }

            return results;
        }

        public async Task<Dictionary<int, List<TimeslotViewModel>>> GetAvailableTimeslotViewModelsForSessions(int? eventId = null)
        {
            var results = new Dictionary<int, List<TimeslotViewModel>>();

            if (!eventId.HasValue)
                eventId = _context.Events.Where(e => e.IsActive).FirstOrDefault().EventId;

            foreach (var session in _context.Sessions)
            {
                var currentlyAssignedTrackId
                    = session != null ? session.TrackId.HasValue ? session.TrackId.Value : 0 : 0;

                var currentlyAssignedTimeslotId
                    = session != null ? session.TimeslotId.HasValue ? session.TimeslotId.Value : 0 : 0;

                // The assigned timeslots for the specified track
                var timeslotsAssignedForTrack = from item in _context.Sessions.Include(s => s.Track)
                                                where item.TrackId == currentlyAssignedTrackId && item.Track.EventId == eventId
                                                select item.TimeslotId;

                // The available timeslots for the specified track
                var availableTimeslotViewModelsForTrack = from timeslot in _context.Timeslots
                                                          where timeslotsAssignedForTrack.Contains(timeslot.TimeslotId) == false
                                                          || timeslot.TimeslotId == currentlyAssignedTimeslotId
                                                          select new TimeslotViewModel
                                                          {
                                                              TimeslotId = timeslot.TimeslotId,
                                                              DisplayName = string.Format("{0:HH:mm:ss} - {1:HH:mm:ss}", timeslot.StartTime, timeslot.EndTime),
                                                              StartTime = timeslot.StartTime,
                                                              EndTime = timeslot.EndTime,
                                                              ContainsNoSessions = timeslot.ContainsNoSessions
                                                          };

                availableTimeslotViewModelsForTrack = availableTimeslotViewModelsForTrack
                    .OrderBy(t => t.StartTime);

                results.Add(session.SessionId, await availableTimeslotViewModelsForTrack.ToListAsync());
            }

            return results;
        }
    }
}
