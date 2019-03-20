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
    public interface ITrackBusinessLogic
    {
        Task<List<Track>> GetAllTracks();
        Task<List<Track>> GetAllTracks(int eventId);
        Task<List<TrackViewModel>> GetAllTrackViewModels(int eventId);
        Task<List<Track>> GetAllTracksForActiveEvent();
        Task<List<TrackViewModel>> GetAllTrackViewModelsForActiveEvent();
        Task<Track> GetTrack(int trackId);
        Task<bool> TrackExists(int trackId);
        Task<bool> CreateTrack(Track track);
        Task<bool> UpdateTrack(Track track);
        Task<bool> DeleteTrack(int trackId);
    }

    public class TrackBusinessLogic : ITrackBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public TrackBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<Track>> GetAllTracks()
        {
            return await _context.Tracks
                .OrderBy(t => t.RoomNumber)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<Track>> GetAllTracks(int eventId)
        {
            return await _context.Tracks
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.RoomNumber)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<List<TrackViewModel>> GetAllTrackViewModels(int eventId)
        {
            var tracks = await _context.Tracks
                .Where(t => t.EventId == eventId)
                .OrderBy(t => t.Name)
                .ThenBy(t => t.RoomNumber)
                .Select(t => new TrackViewModel
                {
                    TrackId = t.TrackId,
                    DisplayName = string.Format("{0} (Room {1})", t.Name, t.RoomNumber),
                    Name = t.Name,
                    RoomNumber = t.RoomNumber
                })
                .ToListAsync();

            return tracks;
        }

        public async Task<List<Track>> GetAllTracksForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if (activeEvent == null)
                return await _context.Tracks
                    .OrderBy(t => t.RoomNumber)
                    .ThenBy(t => t.Name)
                    .ToListAsync();
            else
                return await _context.Tracks
                    .Where(t => t.EventId == activeEvent.EventId)
                    .OrderBy(t => t.RoomNumber)
                    .ThenBy(t => t.Name)
                    .ToListAsync();
        }
        public async Task<List<TrackViewModel>> GetAllTrackViewModelsForActiveEvent()
        {
            var activeEvent
                = await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);

            if(activeEvent?.EventId != null)
            {
                return await GetAllTrackViewModels(activeEvent.EventId);
            }

            return new List<TrackViewModel>();
        }

        public async Task<Track> GetTrack(int trackId)
        {
            return await _context.Tracks
                .FirstOrDefaultAsync(t => t.TrackId == trackId);
        }

        public async Task<bool> TrackExists(int trackId)
        {
            return await _context.Tracks
                .AnyAsync(t => t.TrackId == trackId);
        }

        public async Task<bool> CreateTrack(Track track)
        {
            try
            {
                _context.Tracks.Add(track);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTrack(Track track)
        {
            try
            {
                _context.Tracks.Update(track);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteTrack(int trackId)
        {
            try
            {
                var track = await _context.Tracks.FindAsync(trackId);
                if (track != null)
                {
                    _context.Tracks.Remove(track);

                    await _context.SaveChangesAsync();
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
