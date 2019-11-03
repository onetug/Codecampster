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

    public interface IAnnouncementBusinessLogic
    {
        Task<List<Announcement>> GetAllAnnouncements();
        Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModel();
        Task<Announcement> GetAnnouncement(int announcementId);
        Task<AnnouncementViewModel> GetAnnouncementViewModel(int announcementId);
        Task<List<Announcement>> GetAllAnnouncementsForEvent(int eventId);
        Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModelForEvent(int eventId);
        Task<List<Announcement>> GetAllAnnouncementsForActiveEvent();
        Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModelForActiveEvent();
        Task<List<Announcement>> GetActiveAnnouncementsForEvent(int eventId);
        Task<List<AnnouncementViewModel>> GetActiveAnnouncementsViewModelForEvent(int eventId);
        Task<List<Announcement>> GetActiveAnnouncementsForActiveEvent();
        Task<List<AnnouncementViewModel>> GetActiveAnnouncementsViewModelForActiveEvent();
        Task<bool> AnnouncementExists(int announcementId);
        Task<bool> CreateAnnouncement(Announcement announcement);
        Task<bool> UpdateAnnouncement(Announcement announcement);
        Task<bool> DeleteAnnouncement(int announcementId);
    }

    public class AnnouncementBusinessLogic : IAnnouncementBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public AnnouncementBusinessLogic(
            CodecampDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get all announcements
        /// Order by publish date and then by rank
        /// </summary>
        /// <returns>The collection of Announcement object</returns>
        public async Task<List<Announcement>> GetAllAnnouncements()
        {
            return await _context.Announcements
                // Order by PublishOn date
                .OrderBy(a => a.PublishOn)
                // Then order by rank
                .ThenBy(a => a.Rank)
                .ToListAsync();
        }

        public async Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModel()
        {
            return await ToAnnouncementViewModel(
                _context.Announcements
                .OrderBy(a => a.PublishOn)
                .ThenBy(a => a.Rank))
                .ToListAsync();
        }

        /// <summary>
        /// Get the specified announcement
        /// </summary>
        /// <param name="announcementId">The desired announcement Id</param>
        /// <returns>The collection of Announcement objects</returns>
        public async Task<Announcement> GetAnnouncement(int announcementId)
        {
            return await _context.Announcements.FindAsync(announcementId);
        }

        /// <summary>
        /// Get the speified announcement
        /// </summary>
        /// <param name="announcementId">The desired announcement Id</param>
        /// <returns>The AnnouncementViewModel object</returns>
        public async Task<AnnouncementViewModel> GetAnnouncementViewModel(int announcementId)
        {
            return ToAnnouncementViewModel(
                await _context
                .Announcements
                .FindAsync(announcementId));
        }

        /// <summary>
        /// Get all the announcements for the specified event
        /// Order by publish date then rank
        /// </summary>
        /// <param name="eventId">The desired event Id</param>
        /// <returns>The collection of Announcement objects</returns>
        public async Task<List<Announcement>> GetAllAnnouncementsForEvent(int eventId)
        {
            return await _context.Announcements
                // For the specified event
                .Where(a => a.EventId == eventId)
                // ORder by PublishOnDate
                .OrderBy(a => a.PublishOn)
                // Then by rank
                .ThenBy(a => a.Rank)
                .ToListAsync();
        }

        /// <summary>
        /// Get all announcements for the specified event
        /// </summary>
        /// <param name="eventId">The desired event Id</param>
        /// <returns>The collection of AnnouncementViewModel objects</returns>
        public async Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModelForEvent(
            int eventId)
        {
            return await ToAnnouncementViewModel(
                _context.Announcements
                .Where(a => a.EventId == eventId)
                .OrderBy(a => a.PublishOn)
                .ThenBy(a => a.Rank)
                ).ToListAsync();
        }

        /// <summary>
        /// Get all announcements for the active event
        /// Order by publish date descending then by rank 
        /// </summary>
        /// <returns></returns>
        public async Task<List<Announcement>> GetAllAnnouncementsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent != null)
                return await GetAllAnnouncementsForEvent(activeEvent.EventId);
            else
                // There is no event, return an empty object
                return new List<Announcement>();
        }

        public async Task<List<AnnouncementViewModel>> GetAllAnnouncementsViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent != null)
                return await ToAnnouncementViewModel(
                    _context.Announcements
                    .Where(a => a.EventId == activeEvent.EventId)
                    .OrderBy(a => a.PublishOn)
                    .ThenBy(a => a.Rank)
                    ).ToListAsync();
            else
                // There is no event, return an empty object
                return new List<AnnouncementViewModel>();
        }

        /// <summary>
        /// Get all active announcements for the specified event
        /// Order by PublishOn descending then rank
        /// </summary>
        /// <param name="eventId">The specified event Id</param>
        /// <returns>Collection of Announcement objects</returns>
        public async Task<List<Announcement>> GetActiveAnnouncementsForEvent(int eventId)
        {
            return await _context.Announcements
                // Where the current date time is between
                // the PublishOn and ExpiresOn
                .Where(a => a.EventId == eventId
                && DateTime.Now > a.PublishOn
                && DateTime.Now < a.ExpiresOn)
                // Order by PublishOn date descending
                .OrderByDescending(a => a.PublishOn)
                // Then by rank
                .ThenBy(a => a.Rank)
                .ToListAsync();
        }

        public async Task<List<AnnouncementViewModel>> GetActiveAnnouncementsViewModelForEvent(int eventId)
        {
            return await ToAnnouncementViewModel(
                _context.Announcements
                .Where(a => a.EventId == eventId
                && DateTime.Now > a.PublishOn
                && DateTime.Now < a.ExpiresOn)
                .OrderByDescending(a => a.PublishOn)
                .ThenBy(a => a.Rank)
                ).ToListAsync();
        }

        /// <summary>
        /// Get active announcements for the active event
        /// Order by PublishOn descending and then rank
        /// </summary>
        /// <returns>The collection of Announcement objects</returns>
        public async Task<List<Announcement>> GetActiveAnnouncementsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent != null)
                return await GetActiveAnnouncementsForEvent(activeEvent.EventId);
            else
                return new List<Announcement>();
        }

        public async Task<List<AnnouncementViewModel>> GetActiveAnnouncementsViewModelForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent != null)
                return await ToAnnouncementViewModel(
                    _context.Announcements
                    .Where(a => a.EventId == activeEvent.EventId
                    && DateTime.Now > a.PublishOn
                    && DateTime.Now < a.ExpiresOn)
                    .OrderByDescending(a => a.PublishOn)
                    .ThenBy(a => a.Rank)
                    ).ToListAsync();
            else
                return new List<AnnouncementViewModel>();
        }

        /// <summary>
        /// Determine whether the announcement exists
        /// </summary>
        /// <param name="announcementId">The desired announcement Id</param>
        /// <returns>Indication of whether the announcement exists</returns>
        public async Task<bool> AnnouncementExists(int announcementId)
        {
            return await _context.Announcements.AnyAsync(
                a => a.AnnouncementId == announcementId);
        }

        /// <summary>
        /// Create an announcement
        /// </summary>
        /// <param name="announcement">The announcemnet to create</param>
        /// <returns>Indication of whether the announcement was created</returns>
        public async Task<bool> CreateAnnouncement(Announcement announcement)
        {
            try
            {
                _context.Announcements.Add(announcement);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Update the specified announcement
        /// </summary>
        /// <param name="announcement">The updated announcement information</param>
        /// <returns>Indication of whether the update was successful</returns>
        public async Task<bool> UpdateAnnouncement(Announcement announcement)
        {
            try
            {
                _context.Announcements.Update(announcement);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await AnnouncementExists(announcement.AnnouncementId))
                    return false;
                else
                    throw;
            }
        }

        public async Task<bool> DeleteAnnouncement(int announcementId)
        {
            try
            {
                var announcement = await _context.Announcements.FindAsync(announcementId);
                if (announcement != null)
                {
                    _context.Announcements.Remove(announcement);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private IQueryable<AnnouncementViewModel> ToAnnouncementViewModel(
            IQueryable<Announcement> announcements)
        {
            var resultingAnnouncements
                = from announcement in announcements
                  join _event in _context.Events on announcement.EventId equals _event.EventId
                  select new AnnouncementViewModel
                  {
                      AnnouncementId = announcement.AnnouncementId,
                      Message = announcement.Message,
                      Rank = announcement.Rank,
                      PublishOn = announcement.PublishOn,
                      ExpiresOn = announcement.ExpiresOn,
                      EventName = _event.Name
                  };

            return resultingAnnouncements;
        }

        private AnnouncementViewModel ToAnnouncementViewModel(
            Announcement announcement)
        {
            var _event = _context.Events.FirstOrDefault(
                e => e.EventId == announcement.EventId);

            var result = new AnnouncementViewModel
            {
                AnnouncementId = announcement.AnnouncementId,
                Message = announcement.Message,
                Rank = announcement.Rank,
                PublishOn = announcement.PublishOn,
                ExpiresOn = announcement.ExpiresOn,
                EventName = _event != null ? _event.Name : string.Empty
            };

            return result;
        }
    }
}
