using Codecamp.Data;
using Codecamp.Models.Api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic.Api
{
    public interface IEventsApiBusinessLogic
    {
        Task<List<ApiEvent>> GetEventsList();

        Task<ApiEvent> GetEvent(int eventId);
    }

    public class EventsApiBusinessLogic : ApiBusinessLogic, IEventsApiBusinessLogic
    {
        public EventsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public async Task<List<ApiEvent>> GetEventsList()
        {
            // Get list of web events as API events and convert to JSON
            var apiEventsList =
                await Context.Events.Select(e => new ApiEvent(e)).ToListAsync();

            return apiEventsList;
        }

        public async Task<ApiEvent> GetEvent(int eventId)
        {
            // Get web event as API event and convert to JSON
            var webEvent = await Context.Events.FindAsync(eventId);

            if (webEvent == null)
                return null;

            var apiEvent = new ApiEvent(webEvent);

            return apiEvent;
        }
    }
}