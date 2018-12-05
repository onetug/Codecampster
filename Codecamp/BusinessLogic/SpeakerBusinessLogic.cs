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
    public interface ISpeakerBusinessLogic
    {
        Task<List<Speaker>> GetAllSpeakers();
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModel();
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
        private ISessionBusinessLogic _sessionBL;

        public SpeakerBusinessLogic(CodecampDbContext context,
            ISessionBusinessLogic sessionBL)
        {
            _context = context;
            _sessionBL = sessionBL;
        }

        public async Task<List<Speaker>> GetAllSpeakers()
        {
            return await _context.Speakers
                .Include(s => s.CodecampUser)
                .ToListAsync();
        }

        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModel()
        {
            return await ToSpeakerViewModel(
                _context.Speakers).ToListAsync();
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

        private IQueryable<SpeakerViewModel> ToSpeakerViewModel(IQueryable<Speaker> speakers)
        {
            return from speaker in speakers
                   join codecampUser in _context.CodecampUsers on speaker.CodecampUserId equals codecampUser.Id
                   join speakerSession in _context.SpeakerSessions on speaker.SpeakerId equals speakerSession.SpeakerId into speakerSessionGroupJoin
                   from speakerSession in speakerSessionGroupJoin
                   join session in _context.Sessions on speakerSession.SessionId equals session.SessionId into sessionGroupJoin
                   select new SpeakerViewModel
                   {
                       SpeakerId = speaker.SpeakerId,
                       CodecampUserId = speaker.CodecampUserId,
                       FirstName = codecampUser.FirstName,
                       LastName = codecampUser.LastName,
                       FullName = codecampUser.FullName,
                       CompanyName = speaker.CompanyName,
                       Image = speaker.Image,
                       Bio = speaker.Bio,
                       WebsiteUrl = speaker.WebsiteUrl,
                       BlogUrl = speaker.BlogUrl,
                       GeographicLocation = codecampUser.GeographicLocation,
                       TwitterHandle = codecampUser.TwitterHandle,
                       LinkedIn = speaker.LinkedIn,
                       IsVolunteer = codecampUser.IsVolunteer,
                       IsMvp = speaker.IsMvp,
                       NoteToOrganizers = speaker.NoteToOrganizers,
                       Email = codecampUser.Email,
                       PhoneNumber = codecampUser.PhoneNumber,
                       IsApproved = speaker.IsApproved,
                       Sessions = _sessionBL.ToSessionViewModel(sessionGroupJoin.AsQueryable())
                   };
        }
    }
}
