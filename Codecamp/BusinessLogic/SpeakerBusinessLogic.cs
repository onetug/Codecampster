using Codecamp.Data;
using Codecamp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface ISpeakerBusinessLogic
    {
        Task<List<Speaker>> GetAllSpeakers();
        Task<List<Speaker>> GetAllSpeakers(int eventId);
        Task<List<Speaker>> GetAllApprovedSpeakersForActiveEvent();
        Task<List<Speaker>> GetAllSpeakersForActiveEvent();
        Task<Speaker> GetSpeaker(int speakerId);
        Task<bool> SpeakerExists(int speakerId);
        Task<bool> UpdateSpeaker(Speaker speaker);
        Task<CodecampUser> GetUserInfoForSpeaker(int speakerId);
    }

    public class SpeakerBusinessLogic : ISpeakerBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public SpeakerBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<Speaker>> GetAllSpeakers()
        {
            return await _context.Speakers
                .Include(s => s.CodecampUser)
                .ToListAsync();
        }

        public async Task<List<Speaker>> GetAllApprovedSpeakersForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Speakers.Include(s => s.CodecampUser)
                    .ToListAsync();
            else
                return await _context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId
                    && s.IsApproved == true)
                    .ToListAsync();
        }

        public async Task<List<Speaker>> GetAllSpeakers(int eventId)
        {
            return await _context.Speakers.Include(s => s.CodecampUser)
                .Where(s => s.CodecampUser.EventId == eventId)
                .ToListAsync();
        }

        public async Task<List<Speaker>> GetAllSpeakersForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Speakers.Include(s => s.CodecampUser)
                    .ToListAsync();
            else
                return await _context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId)
                    .ToListAsync();
        }

        public async Task<Speaker> GetSpeaker(int speakerId)
        {
            return await _context.Speakers.Include(s => s.CodecampUser)
                .FirstOrDefaultAsync(s => s.SpeakerId == speakerId);
        }

        public async Task<bool> SpeakerExists(int speakerId)
        {
            return await _context.Speakers.AnyAsync(s => s.SpeakerId == speakerId);
        }

        public async Task<bool> UpdateSpeaker(Speaker speaker)
        {
            try
            {
                _context.Speakers.Update(speaker);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await SpeakerExists(speaker.SpeakerId))
                    return false;
                else
                    throw;
            }
        }
        public async Task<CodecampUser> GetUserInfoForSpeaker(int speakerId)
        {
            var speaker = await _context.Speakers
                .FirstOrDefaultAsync(s => s.SpeakerId == speakerId);

            return speaker != null ? await _context.CodecampUsers
                .FirstOrDefaultAsync(c => c.Id == speaker.CodecampUserId)
                : null;
        }
    }
}
