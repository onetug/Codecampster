using Codecamp.Data;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface ISessionBusinessLogic
    {
        Task<List<Session>> GetAllSessions();
        Task<List<SessionViewModel>> GetAllSessionsViewModel(string userId = null);
        Task<List<Session>> GetAllSessions(int eventId);
        Task<List<SessionViewModel>> GetAllSessionsViewModel(int eventId, string userId = null);
        Task<List<Session>> GetAllSessionsForActiveEvent();
        Task<List<SessionViewModel>> GetAllSessionsViewModelForActiveEvent(string userId = null);
        Task<List<Session>> GetAllApprovedSessions(int eventId);
        Task<List<SessionViewModel>> GetAllApprovedSessionsViewModel(int eventId,
            string userId = null);
        Task<List<Session>> GetAllApprovedSessionsForActiveEvent();
        Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForActiveEvent(
            string userId = null);
        Task<List<Session>> GetAllSessionsForSpeakerForActiveEvent(int speakerId);
        Task<List<SessionViewModel>> GetAllSessionsViewModelForSpeakerForActiveEvent(
            int speakerId, string userId = null);
        Task<List<Session>> GetAllApprovedSessionsForSpeakerForActiveEvent(int speakerId);
        Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForSpeakerForActiveEvent(
            int speakerId, string userId = null);
        Task<Session> GetSession(int sessionId);
        Task<SessionViewModel> GetSessionViewModel(int sessionId, string userId = null);
        Task<bool> CreateSession(Session session, int speakerId);
        Task<bool> SessionExists(int sessionId);
        Task<bool> SpeakerSessionExists(int speakerId, int sessionId);
        Task<bool> UpdateSession(Session session);
        Task<bool> DeleteSession(int sessionId);
        Task<bool> ToggleFavoriteSession(int sessionId, string userId);
        IQueryable<SessionViewModel> ToSessionViewModel(IQueryable<Session> sessions,
            string userId = null);
    }

    public class SessionBusinessLogic : ISessionBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public SessionBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<Session>> GetAllSessions()
        {
            return await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessionsViewModel(string userId = null)
        {
            var sessionViewModels = await ToSessionViewModel(
                _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event), userId)
                .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<Session>> GetAllSessions(int eventId)
        {
            return await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .Where(s => s.EventId == eventId)
                .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessionsViewModel(int eventId, string userId = null)
        {
            var sessionViewModels = await ToSessionViewModel(
                _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .Where(s => s.EventId == eventId), userId)
                .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<Session>> GetAllSessionsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.EventId == activeEvent.EventId)
                    .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessionsViewModelForActiveEvent(string userId = null)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            var sessionViewModels = new List<SessionViewModel>();
            if (activeEvent == null)
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event), userId)
                    .ToListAsync();

            else
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.EventId == activeEvent.EventId), userId)
                    .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<Session>> GetAllApprovedSessions(int eventId)
        {
            return await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .Where(s => s.EventId == eventId
                    && s.IsApproved == true)
                .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllApprovedSessionsViewModel(int eventId,
            string userId = null)
        {
            var sessionViewModels = await ToSessionViewModel(
                _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .Where(s => s.EventId == eventId && s.IsApproved == true), userId)
                .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<Session>> GetAllApprovedSessionsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.IsApproved == true)
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.EventId == activeEvent.EventId
                        && s.IsApproved == true)
                    .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForActiveEvent(
            string userId = null)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            var sessionViewModels = new List<SessionViewModel>();
            if (activeEvent == null)
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.IsApproved == true), userId)
                    .ToListAsync();
            else
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.EventId == activeEvent.EventId && s.IsApproved == true), userId)
                    .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<Session>> GetAllSessionsForSpeakerForActiveEvent(int speakerId)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId))
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId)
                    .ToListAsync();
        }

        public async Task<List<Session>> GetAllApprovedSessionsForSpeakerForActiveEvent(int speakerId)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId) && s.IsApproved == true)
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId && s.IsApproved == true)
                    .ToListAsync();
        }

        public async Task<List<SessionViewModel>> GetAllSessionsViewModelForSpeakerForActiveEvent(
            int speakerId, string userId = null)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            var sessionViewModels = new List<SessionViewModel>();
            if (activeEvent == null)
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)), userId)
                    .ToListAsync();
            else
                sessionViewModels =  await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId), userId)
                    .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        public async Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForSpeakerForActiveEvent(
            int speakerId, string userId = null)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            var sessionViewModels = new List<SessionViewModel>();
            if (activeEvent == null)
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId && s.IsApproved == true)),
                    userId)
                    .ToListAsync();
            else
                sessionViewModels = await ToSessionViewModel(
                    _context.Sessions
                    .Include(s => s.SpeakerSessions)
                    .Include(s => s.AttendeeSessions)
                    .Include(s => s.Event)
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId && s.IsApproved == true), userId)
                    .ToListAsync();

            for (int index = 0; index < sessionViewModels.Count(); index++)
            {
                var speakerSessions
                    = _context.SpeakerSessions
                    .Include(ss => ss.Session)
                    .Include(ss => ss.Speaker)
                    .Include(ss => ss.Speaker.CodecampUser)
                    .Where(ss => ss.SessionId == sessionViewModels[index].SessionId);

                sessionViewModels[index].SpeakerSessions = speakerSessions.ToList();
            }

            return sessionViewModels;
        }

        /// <summary>
        /// Get the specified session
        /// </summary>
        /// <param name="sessionId">The specified session Id</param>
        /// <returns>The Session object</returns>
        public async Task<Session> GetSession(int sessionId)
        {
            return await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        /// <summary>
        /// Get the specified session
        /// </summary>
        /// <param name="sessionId">The specified session Id</param>
        /// <returns>The SessionViewModel object</returns>
        public async Task<SessionViewModel> GetSessionViewModel(int sessionId, string userId = null)
        {
            return ToSessionViewModel(await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId), userId);
        }

        /// <summary>
        /// Create the specified session for the specified speaker
        /// </summary>
        /// <param name="session">The session to create</param>
        /// <param name="speakerId">The speaker Id</param>
        /// <returns>True/False whether the session was created</returns>
        public async Task<bool> CreateSession(Session session, int speakerId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    _context.Sessions.Add(session);

                    // Save the session
                    await _context.SaveChangesAsync();

                    // Now add the current user as a speaker
                    _context.SpeakerSessions.Add(new SpeakerSession { SpeakerId = speakerId, SessionId = session.SessionId });

                    await _context.SaveChangesAsync();

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Determines whether the specified session exists
        /// </summary>
        /// <param name="sessionId">The speified session Id</param>
        /// <returns>True/False of whether the sssion exists</returns>
        public async Task<bool> SessionExists(int sessionId)
        {
            return await _context.Sessions.AnyAsync(s => s.SessionId == sessionId);
        }
        
        /// <summary>
        /// Determines whether the specified speaker/session exists
        /// </summary>
        /// <param name="speakerId">The specified speaker Id</param>
        /// <param name="sessionId">The specified session Id</param>
        /// <returns>True/False of whether the speaker/session exists</returns>
        public async Task<bool> SpeakerSessionExists(int speakerId, int sessionId)
        {
            return await _context.SpeakerSessions.AnyAsync(
                ss => ss.SpeakerId == speakerId && ss.SessionId == sessionId);
        }

        /// <summary>
        /// Update the specified session
        /// </summary>
        /// <param name="session">The Session object with the values to update</param>
        /// <returns>True/False of whether the update was successsful</returns>
        public async Task<bool> UpdateSession(Session session)
        {
            try
            {
                _context.Sessions.Update(session);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                return false;
            }
        }

        /// <summary>
        /// Delete the specified session
        /// </summary>
        /// <param name="sessionId">The specified session Id</param>
        /// <returns>True/False of whether the delete was successful</returns>
        public async Task<bool> DeleteSession(int sessionId)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var session = await _context.Sessions.FindAsync(sessionId);
                    if (session != null)
                    {
                        _context.Sessions.Remove(session);

                        await _context.SaveChangesAsync();
                    }

                    var speakerSessions = _context.SpeakerSessions.Where(ss => ss.SessionId == sessionId);
                    if (speakerSessions != null)
                    {
                        _context.SpeakerSessions.RemoveRange(speakerSessions);

                        await _context.SaveChangesAsync();
                    }

                    transaction.Commit();

                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public async Task<bool> ToggleFavoriteSession(int sessionId, string userId)
        {
            var attendeeSession = await _context.AttendeesSessions
                .Where(_as => _as.CodecampUserId == userId
                && _as.SessionId == sessionId)
                .FirstOrDefaultAsync();
            if (attendeeSession == null)
            {
                try
                {
                    // Add the attendee session
                    _context.AttendeesSessions
                        .Add(new AttendeeSession
                        {
                            SessionId = sessionId,
                            CodecampUserId = userId
                        });

                    // Save changes
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
            else
            {
                try
                {
                    // Remove the attendee session
                    _context
                        .AttendeesSessions
                        .Remove(attendeeSession);

                    // Save changes
                    await _context.SaveChangesAsync();

                    return true;
                }
                catch (DbUpdateConcurrencyException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Convert a Session to a SessionViewModel
        /// </summary>
        /// <param name="sessions">IQueryable<Session> object</param>
        /// <returns>IQueryable<SessionViewModel> object</returns>
        public IQueryable<SessionViewModel> ToSessionViewModel(IQueryable<Session> sessions,
            string userId = null)
        {
            if (sessions == null) return null;

            var sessionViewModels = from session in sessions
                                    select new SessionViewModel
                                    {
                                        SessionId = session.SessionId,
                                        Name = session.Name,
                                        Description = session.Description,
                                        SkillLevel = SkillLevel.GetSkillLevelDescription(session.SkillLevel),
                                        Keywords = session.Keywords,
                                        IsApproved = session.IsApproved,
                                        EventName = session.Event.Name,
                                        IsUserFavorite = userId == null ? false : session.AttendeeSessions
                                            .Exists(_as => _as.CodecampUserId == userId
                                            && _as.SessionId == session.SessionId)
                                    };

            return sessionViewModels;
        }

        public SessionViewModel ToSessionViewModel(Session session, string userId = null)
        {
            if (session == null) return null;

            var speakerSessions = _context.SpeakerSessions
                .Include(ss => ss.Session)
                .Include(ss => ss.Speaker)
                .Include(ss => ss.Speaker.CodecampUser)
                .Where(ss => ss.SessionId == session.SessionId)
                .ToList();

            var sessionViewModel = new SessionViewModel
            {
                SessionId = session.SessionId,
                Name = session.Name,
                Description = session.Description,
                SkillLevel = SkillLevel.GetSkillLevelDescription(session.SkillLevel),
                Keywords = session.Keywords,
                IsApproved = session.IsApproved,
                SpeakerSessions = speakerSessions,
                EventName = session.Event.Name,
                IsUserFavorite = userId == null ? false : session.AttendeeSessions
                    .Exists(_as => _as.CodecampUserId == userId
                    && _as.SessionId == session.SessionId)
            };

            return sessionViewModel;
        }
    }
}
