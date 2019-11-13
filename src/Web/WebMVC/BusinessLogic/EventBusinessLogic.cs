using Codecamp.Data;
using Codecamp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic
{
    public interface IEventBusinessLogic
    {
        Task<List<Event>> GetAllEvents();
        Task<Event> GetEvent(int eventId);
        Task<Event> GetActiveEvent();
        Task<int> SetEventActive(int eventId);
        Task<int> CreateEvent(Event theEvent);
        Task<bool> UpdateEvent(Event theEvent);
        Task<int> DeleteEvent(int eventId);
    }

    public class EventBusinessLogic : IEventBusinessLogic
    {
        private CodecampDbContext _context { get; set; }

        public EventBusinessLogic(CodecampDbContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEvents()
        {
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEvent(int eventId)
        {
            return await _context.Events.FindAsync(eventId);
        }

        public async Task<Event> GetActiveEvent()
        {
            return await _context.Events
                .FirstOrDefaultAsync(e => e.IsActive == true);
        }

        public async Task<int> SetEventActive(int eventId)
        {
            foreach (var _event in _context.Events)
            {
                _event.IsActive = _event.EventId == eventId ? true : false;
            }

            return await _context.SaveChangesAsync();
        }

        public async Task<int> CreateEvent(Event theEvent)
        {
            _context.Events.Add(theEvent);

            if (theEvent.IsActive == false)
                return await _context.SaveChangesAsync();
            else
            {
                var result = await _context.SaveChangesAsync();

                // The save succeded, now set the active event 
                if (result != 0)
                    return await SetEventActive(theEvent.EventId);

                return result;
            }
        }

        public async Task<bool> UpdateEvent(Event theEvent)
        {
            try
            {
                _context.Events.Update(theEvent);

                if (theEvent.IsActive == true)
                    await SetEventActive(theEvent.EventId);

                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await EventExists(theEvent.EventId))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<bool> EventExists(int eventId)
        {
            return await _context.Events.AnyAsync(s => s.EventId == eventId);
        }

        public async Task<int> DeleteEvent(int eventId)
        {
            var theEvent = await _context.Events.FindAsync(eventId);

            if (theEvent == null)
                return 0;

            _context.Events.Remove(theEvent);
            return await _context.SaveChangesAsync();
        }
    }
}
