using Codecamp.Data;
using Codecamp.Models;
using Codecamp.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface ISpeakerBusinessLogic
    {
        Task<List<Speaker>> GetAllSpeakers();
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModel();
        Task<List<Speaker>> GetAllSpeakers(int eventId);
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(int eventId);
        Task<List<Speaker>> GetAllApprovedSpeakersForActiveEvent();
        Task<List<SpeakerViewModel>> GetAllApprovedSpeakersViewModelForActiveEvent();
        Task<List<Speaker>> GetAllSpeakersForActiveEvent();
        Task<List<SpeakerViewModel>> GetAllSpeakersViewModelForActiveEvent();
        Task<Speaker> GetSpeaker(int speakerId);
        Task<SpeakerViewModel> GetSpeakerViewModel(int speakerId, bool onlyApprovedSessions = false);
        Task<bool> SpeakerExists(int speakerId);
        Task<bool> UpdateSpeaker(Speaker speaker);
        Task<CodecampUser> GetUserInfoForSpeaker(int speakerId);
        List<SpeakerViewModel> ToSpeakerViewModel(List<Speaker> speakers);
        SpeakerViewModel ToSpeakerViewModel(Speaker speaker);
        byte[] ResizeImage(int speakerId);
        byte[] ResizeImage(byte[] originalImage);
        Task<List<SpeakerViewModel>> GetSpeakersForSession(int sessionId);

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
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModel()
        {
            return ToSpeakerViewModel(
                await _context.Speakers.ToListAsync());
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
        public async Task<List<SpeakerViewModel>> GetAllApprovedSpeakersViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return ToSpeakerViewModel(await _context.Speakers.Include(s => s.CodecampUser).ToListAsync());
            else
                return ToSpeakerViewModel(await _context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId
                    && s.IsApproved == true).ToListAsync());
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
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModel(int eventId)
        {
            return ToSpeakerViewModel(await _context.Speakers.Include(s => s.CodecampUser)
                .Where(s => s.CodecampUser.EventId == eventId).ToListAsync());
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
        public async Task<List<SpeakerViewModel>> GetAllSpeakersViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return ToSpeakerViewModel(await _context.Speakers.Include(s => s.CodecampUser).ToListAsync());
            else
                return ToSpeakerViewModel(await _context.Speakers.Include(s => s.CodecampUser)
                    .Where(s => s.CodecampUser.EventId == activeEvent.EventId).ToListAsync());
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
            var speaker = _context.Speakers.Include(s => s.CodecampUser)
                .FirstOrDefault(s => s.SpeakerId == speakerId);
            var speakerViewModel = ToSpeakerViewModel(speaker);

            if (onlyApprovedSessions == false)
                speakerViewModel.Sessions = await _sessionBL.GetAllSessionsViewModelForSpeakerForActiveEvent(speakerViewModel.SpeakerId);
            else
                speakerViewModel.Sessions = await _sessionBL.GetAllApprovedSessionsViewModelForSpeakerForActiveEvent(speakerViewModel.SpeakerId);

            // Get the image size for display
            var imageArray = speaker != null ? speaker.Image : null;
            if (imageArray != null)
            {
                MemoryStream imageStream = new MemoryStream(imageArray);                
                using (var image = new Bitmap(imageStream))
                {
                    speakerViewModel.ImageSizePixels
                        = image.Width + " pixels x " + image.Height + " pixels";

                    speakerViewModel.IsImageResizable = (image.Width > 300 || image.Height > 300) ? true : false;
                }
            }

            return speakerViewModel;
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
        public List<SpeakerViewModel> ToSpeakerViewModel(List<Speaker> speakers)
        {
            List<SpeakerViewModel> resultingSpeakers;

            resultingSpeakers = (from speaker in speakers
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
                                }).ToList();

            return resultingSpeakers;
        }

        public SpeakerViewModel ToSpeakerViewModel(Speaker speaker)
        {
            var codecampUser = _context.CodecampUsers.Find(speaker.CodecampUserId);
            var result = new SpeakerViewModel
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
            };

            return result;
        }

        /// <summary>
        /// Resize the image of the speaker Id to 300px X 300px
        /// </summary>
        /// <param name="speakerId">The desired speaker's Id</param>
        /// <returns>The resized image byte[]</returns>
        public byte[] ResizeImage(int speakerId)
        {
            var speaker = _context.Speakers.AsNoTracking().Include(s => s.CodecampUser)
                .FirstOrDefault(s => s.SpeakerId == speakerId);

            // Get the image size for display
            var imageArray = speaker != null ? speaker.Image : null;

            return ResizeImage(imageArray);
        }

        /// <summary>
        /// Resize the supplied image file byte[] to 300px X 300px
        /// </summary>
        /// <param name="originalImage">The original image byte[]</param>
        /// <returns>The resized image byte[]</returns>
        public byte[] ResizeImage(byte[] originalImage)
        {
            const int size = 300; // max size in pixels

            if (originalImage != null)
            {
                MemoryStream imageStream = new MemoryStream(originalImage);
                using (var image = new Bitmap(imageStream))
                {
                    if (image.Width < size && image.Height < size)
                        return originalImage;

                    int width, height;
                    if (image.Width > image.Height)
                    {
                        width = size;
                        height = Convert.ToInt32(image.Height * size / (double)image.Width);
                    }
                    else
                    {
                        width = Convert.ToInt32(image.Width * size / (double)image.Height);
                        height = size;
                    }

                    var resized = new Bitmap(width, height);
                    using (var graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingMode = CompositingMode.SourceCopy;
                        graphics.DrawImage(image, 0, 0, width, height);

                        using (var ms = new MemoryStream())
                        {
                            var qualityParamId = Encoder.Quality;
                            var encoderParameters = new EncoderParameters(1);
                            encoderParameters.Param[0] = new EncoderParameter(qualityParamId, 100L);
                            var codec = ImageCodecInfo.GetImageDecoders()
                                .FirstOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);
                            resized.Save(ms, codec, encoderParameters);

                            return ms.ToArray();
                        }
                    }
                }
            }

            return null;
        }

        public async Task<List<SpeakerViewModel>> GetSpeakersForSession(int sessionId)
        {
            var speakers = ToSpeakerViewModel(
                await _context.Speakers
                .Where(s => _context.SpeakerSessions
                .Where(ss => ss.SessionId == sessionId)
                .Select(s2 => s2.SpeakerId)
                .Contains(s.SpeakerId)).ToListAsync());

            return speakers;
        }
    }
}
