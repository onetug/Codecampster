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
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(bool loadImages = true);
        Task<List<Speaker>> GetAllSpeakers(int eventId);
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(int eventId, bool loadImages = true);
        Task<List<Speaker>> GetAllApprovedSpeakersForActiveEvent();
        Task<List<SpeakerViewModel>> GetAllApprovedSpeakersViewModelForActiveEvent(bool loadImages = true);
        Task<List<Speaker>> GetAllSpeakersForActiveEvent();
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModelForActiveEvent(bool loadImages = true);
        Task<Speaker> GetSpeaker(int speakerId);
        Task<SpeakerViewModel> GetSpeakerViewModel(int speakerId, bool onlyApprovedSessions = false);
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

        /// <summary>
        /// Get all speakers
        /// </summary>
        /// <returns>List of Speaker objects</returns>
        public async Task<List<Speaker>> GetAllSpeakers()
        {
            return await _context.Speakers
                .Include(s => s.CodecampUser)
                .ToListAsync();
        }

        /// <summary>
        /// Get list of all speakers
        /// </summary>
        /// <param name="loadImages">Indicates whether to load the speaker images in the results</param>
        /// <returns>List of SpeakerViewModel objects</returns>
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(bool loadImages = true)
        {
            return await ToSpeakerViewModel(
                _context.Speakers, loadImages).ToListAsync();
        }

        /// <summary>
        /// Get all approved speakers for the active event
        /// </summary>
        /// <returns>List of approved Speaker objects</returns>
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

        /// <summary>
        /// Get list of approved speakers for the active event
        /// </summary>
        /// <param name="loadImages">Indicates whether to load the speaker images in the results</param>
        /// <returns>List of SpeakerViewModel objects</returns>
        public async Task<List<SpeakerViewModel>> GetAllApprovedSpeakersViewModelForActiveEvent(bool loadImages = true)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser), loadImages)
                    .ToListAsync();
            else
                return await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId
                    && s.IsApproved == true), loadImages)
                    .ToListAsync();
        }

        /// <summary>
        /// Get all speakers for the specified event
        /// </summary>
        /// <param name="eventId">The desired event Id</param>
        /// <returns>List of Speaker objects</returns>
        public async Task<List<Speaker>> GetAllSpeakers(int eventId)
        {
            return await _context.Speakers.Include(s => s.CodecampUser)
                .Where(s => s.CodecampUser.EventId == eventId)
                .ToListAsync();
        }

        /// <summary>
        /// Get all speakers for the specified event
        /// </summary>
        /// <param name="eventId">The desired evetn Id</param>
        /// <param name="loadImages">Indicates whether to load the speaker image in the results</param>
        /// <returns>List of SpeakerViewModel objects</returns>
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(int eventId, bool loadImages = true)
        {
            return await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser)
                .Where(s => s.CodecampUser.EventId == eventId), loadImages)
                .ToListAsync();
        }

        /// <summary>
        /// Get all speakers for the active event
        /// </summary>
        /// <returns>List of Speaker objects</returns>
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

        /// <summary>
        /// Get all speakers for the active event
        /// </summary>
        /// <param name="loadImages">Indicates whether to load the speaker image in the results</param>
        /// <returns>List of SpeakerViewModel objects</returns>
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModelForActiveEvent(bool loadImages = true)
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser), loadImages)
                    .ToListAsync();
            else
                return await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId), loadImages)
                    .ToListAsync();
        }

        /// <summary>
        /// Get the specified speaker
        /// </summary>
        /// <param name="speakerId">The desired speaker Id</param>
        /// <returns>The Speaker object</returns>
        public async Task<Speaker> GetSpeaker(int speakerId)
        {
            return await _context.Speakers.Include(s => s.CodecampUser)
                .FirstOrDefaultAsync(s => s.SpeakerId == speakerId);
        }

        /// <summary>
        /// Get the specified speaker
        /// </summary>
        /// <param name="speakerId">The desired speaker Id</param>
        /// <param name="onlyApprovedSessions">Indicates whether to load only approved sessions or all sessions</param>
        /// <returns>The SpeakerViewModel object</returns>
        public async Task<SpeakerViewModel> GetSpeakerViewModel(int speakerId, bool onlyApprovedSessions = false)
        {
            var speaker = await ToSpeakerViewModel(_context.Speakers.Include(s => s.CodecampUser)
                .Where(s => s.SpeakerId == speakerId)).FirstOrDefaultAsync();

            if (onlyApprovedSessions == false)
                speaker.Sessions = await _sessionBL.GetAllSessionsViewModelForSpeakerForActiveEvent(speaker.SpeakerId);
            else
                speaker.Sessions = await _sessionBL.GetAllApprovedSessionsViewModelForSpeakerForActiveEvent(speaker.SpeakerId);

            return speaker;
        }

        /// <summary>
        /// Determines whether the speaker exists 
        /// </summary>
        /// <param name="speakerId">The desired speaker Id</param>
        /// <returns>True/False of whether the speaker exists</returns>
        public async Task<bool> SpeakerExists(int speakerId)
        {
            return await _context.Speakers.AnyAsync(s => s.SpeakerId == speakerId);
        }

        /// <summary>
        /// Update the specified speaker
        /// </summary>
        /// <param name="speaker">The Speaker object with the values to update</param>
        /// <returns>Indicates whether the update was successful</returns>
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

        /// <summary>
        /// Get the User informtaion for the specified speaker
        /// </summary>
        /// <param name="speakerId">The desired speaker Id</param>
        /// <returns>The corresponding CodecampUser object</returns>
        public async Task<CodecampUser> GetUserInfoForSpeaker(int speakerId)
        {
            var speaker = await _context.Speakers
                .FirstOrDefaultAsync(s => s.SpeakerId == speakerId);

            return speaker != null ? await _context.CodecampUsers
                .FirstOrDefaultAsync(c => c.Id == speaker.CodecampUserId)
                : null;
        }

        /// <summary>
        /// Converts a IQueryable<Speaker> object to a IQueryable<SpeakerViewModel> object
        /// </summary>
        /// <param name="speakers">The IQueryable<Speaker> object</param>
        /// <param name="loadImages">Indicates whether to load the Speaker image in the results</param>
        /// <returns>IQueryable<SpeakerViewModel> object</returns>
        private IQueryable<SpeakerViewModel> ToSpeakerViewModel(IQueryable<Speaker> speakers, bool loadImages = true)
        {
            return from speaker in speakers
                   join codecampUser in _context.CodecampUsers on speaker.CodecampUserId equals codecampUser.Id
                   select new SpeakerViewModel
                   {
                       SpeakerId = speaker.SpeakerId,
                       CodecampUserId = speaker.CodecampUserId,
                       FirstName = codecampUser.FirstName,
                       LastName = codecampUser.LastName,
                       FullName = codecampUser.FullName,
                       CompanyName = speaker.CompanyName,
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
                       Image = loadImages == true
                        ? speaker.Image == null || (speaker.Image != null && speaker.Image.Length > SpeakerViewModel.MaxImageSize)
                        ? string.Empty : String.Format("data:image;base64,{0}", Convert.ToBase64String(speaker.Image))
                        : string.Empty
                   };
        }
    }
}
