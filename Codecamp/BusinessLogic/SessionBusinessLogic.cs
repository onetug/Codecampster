using Codecamp.Data;
using Codecamp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface ISessionBusinessLogic
    {
        Task<List<Session>> GetAllSessions();
        Task<List<Session>> GetAllSessions(int eventId);
        Task<List<Session>> GetAllSessionsForActiveEvent();
        Task<List<Session>> GetAllApprovedSessions(int eventId);
        Task<List<Session>> GetAllApprovedSessionsForActiveEvent();
        Task<List<Session>> GetAllSessionsForSpeakerForActiveEvent(int speakerId);
        Task<Session> GetSession(int sessionId);
        Task<int> CreateSession(Session session);
        Task<bool> SessionExists(int sessionId);
        Task<bool> UpdateSession(Session session);
        Task<int> DeleteSession(int sessionId);
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
                .Include(s => s.Speakers)
                .ToListAsync();
        }

        public async Task<List<Session>> GetAllSessions(int eventId)
        {
            return await _context.Sessions
                .Include(s => s.Speakers)
                .Where(s => s.EventId == eventId)
                .ToListAsync();
        }

        public async Task<List<Session>> GetAllSessionsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .Where(s => s.EventId == activeEvent.EventId)
                    .ToListAsync();
        }

        public async Task<List<Session>> GetAllApprovedSessions(int eventId)
        {
            return await _context.Sessions
                .Include(s => s.Speakers)
                .Where(s => s.EventId == eventId 
                    && s.IsApproved == true)
                .ToListAsync();
        }

        public async Task<List<Session>> GetAllApprovedSessionsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .Where(s => s.IsApproved == true)
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .Where(s => s.EventId == activeEvent.EventId
                        && s.IsApproved == true)
                    .ToListAsync();
        }

        public async Task<List<Session>> GetAllSessionsForSpeakerForActiveEvent(int speakerId)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .Where(s => s.Speakers.Any(s2 => s2.SpeakerId == speakerId))
                    .ToListAsync();
            else
                return await _context.Sessions
                    .Include(s => s.Speakers)
                    .Where(s => s.Speakers.Any(s2 => s2.SpeakerId == speakerId)
                        && s.EventId == activeEvent.EventId)
                    .ToListAsync();
        }

        public async Task<Session> GetSession(int sessionId)
        {
            return await _context.Sessions
                .Include(s => s.Speakers)
                .FirstOrDefaultAsync(s => s.SessionId == sessionId);
        }

        public async Task<int> CreateSession(Session session)
        {
            _context.Sessions.Add(session);

            return await _context.SaveChangesAsync();
        }
        public async Task<bool> SessionExists(int sessionId)
        {
            return await _context.Sessions.AnyAsync(s => s.SessionId == sessionId);
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
                if (!await SessionExists(session.SessionId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<int> DeleteSession(int sessionId)
        {
            var session = await _context.Sessions.FindAsync(sessionId);

            if (session == null)
                return 0;

            _context.Sessions.Remove(session);
            return await _context.SaveChangesAsync();
        }
    }
}
