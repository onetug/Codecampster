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
        Task<List<SessionViewModel>> GetAllSessionsViewModel();
        Task<List<Session>> GetAllSessions(int eventId);
        Task<List<SessionViewModel>> GetAllSessionsViewModel(int eventId);
        Task<List<Session>> GetAllSessionsForActiveEvent();
        Task<List<SessionViewModel>> GetAllSessionsViewModelForActiveEvent();
        Task<List<Session>> GetAllApprovedSessions(int eventId);
        Task<List<SessionViewModel>> GetAllApprovedSessionsViewModel(int eventId);
        Task<List<Session>> GetAllApprovedSessionsForActiveEvent();
        Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForActiveEvent();
        Task<List<Session>> GetAllSessionsForSpeakerForActiveEvent(int speakerId);
        Task<List<SessionViewModel>> GetAllSessionsViewModelForSpeakerForActiveEvent(int speakerId);
        Task<Session> GetSession(int sessionId);
        Task<SessionViewModel> GetSessionViewModel(int sessionId);
        Task<bool> CreateSession(Session session, int speakerId);
        Task<bool> SessionExists(int sessionId);
        Task<bool> SpeakerSessionExists(int speakerId, int sessionId);
        Task<bool> UpdateSession(Session session);
        Task<bool> DeleteSession(int sessionId);
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

        public async Task<List<SessionViewModel>> GetAllSessionsViewModel()
        {
            return await ToSessionViewModel(_context.Sessions).ToListAsync();
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

        public async Task<List<SessionViewModel>> GetAllSessionsViewModel(int eventId)
        {
            return await ToSessionViewModel(_context.Sessions
                .Where(s => s.EventId == eventId))
                .ToListAsync();
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

        public async Task<List<SessionViewModel>> GetAllSessionsViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await ToSessionViewModel(_context.Sessions)
                    .ToListAsync();
            else
                return await ToSessionViewModel(_context.Sessions
                    .Where(s => s.EventId == activeEvent.EventId))
                    .ToListAsync();
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

        public async Task<List<SessionViewModel>> GetAllApprovedSessionsViewModel(int eventId)
        {
            return await ToSessionViewModel(_context.Sessions
                .Where(s => s.EventId == eventId && s.IsApproved == true))
                .ToListAsync();
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

        public async Task<List<SessionViewModel>> GetAllApprovedSessionsViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await ToSessionViewModel(_context.Sessions
                    .Where(s => s.IsApproved == true))
                    .ToListAsync();
            else
                return await ToSessionViewModel(_context.Sessions
                    .Where(s => s.EventId == activeEvent.EventId && s.IsApproved == true))
                    .ToListAsync();
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

        public async Task<List<SessionViewModel>> GetAllSessionsViewModelForSpeakerForActiveEvent(int speakerId)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await ToSessionViewModel(_context.Sessions
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)))
                    .ToListAsync();
            else
                return await ToSessionViewModel(_context.Sessions
                    .Where(s => s.SpeakerSessions.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId))
                    .ToListAsync();
        }

        public async Task<Session> GetSession(int sessionId)
        {
            return await _context.Sessions
                .Include(s => s.SpeakerSessions)
                .Include(s => s.AttendeeSessions)
                .Include(s => s.Event)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        public async Task<SessionViewModel> GetSessionViewModel(int sessionId)
        {
            return await ToSessionViewModel(_context.Sessions)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

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

        public async Task<bool> SessionExists(int sessionId)
        {
            return await _context.Sessions.AnyAsync(s => s.SessionId == sessionId);
        }

        public async Task<bool> SpeakerSessionExists(int speakerId, int sessionId)
        {
            return await _context.SpeakerSessions.AnyAsync(
                ss => ss.SpeakerId == speakerId && ss.SessionId == sessionId);
        }

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

        private IQueryable<SessionViewModel> ToSessionViewModel(IQueryable<Session> sessions)
        {
            return from session in sessions
                   join speakerSession in _context.SpeakerSessions on session.SessionId equals speakerSession.SessionId into speakerSessionsGroupJoin
                   join _event in _context.Events on session.EventId equals _event.EventId
                   from _speakerSession in speakerSessionsGroupJoin
                   join codecampUser in _context.CodecampUsers on _speakerSession.SpeakerId equals codecampUser.SpeakerId into codecampUsersGroupJoin
                   select new SessionViewModel
                   {
                       SessionId = session.SessionId,
                       Name = session.Name,
                       Description = session.Description,
                       SkillLevel = SkillLevel.GetSkillLevelDescription(session.SkillLevel),
                       Keywords = session.Keywords,
                       IsApproved = session.IsApproved,
                       Speakers = string.Join(",", (from codecampUser in codecampUsersGroupJoin select codecampUser.FullName)),
                       SpeakerSessions = session.SpeakerSessions,
                       EventName = _event.Name
                   };
        }

    }
}
