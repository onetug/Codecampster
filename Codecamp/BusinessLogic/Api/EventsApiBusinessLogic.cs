using Codecamp.Data;
using Codecamp.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface IEventsApiBusinessLogic
    {
        List<ApiEvent> GetEventsList();

        ApiEvent GetEvent(int eventId);
    }

    public class EventsApiBusinessLogic : ApiBusinessLogic, IEventsApiBusinessLogic
    {
        public EventsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiEvent> GetEventsList()
        {
            // Get list of web events as API events and convert to JSON
            var apiEventsList = Context.Events
                .Select(e => new ApiEvent(e))
                .ToList();

            return apiEventsList;
        }

        public ApiEvent GetEvent(int eventId)
        {
            // Get web event as API event and convert to JSON
            var webEvent = Context.Events.Find(eventId);

            if (webEvent == null)
                return null;

            var apiEvent = new ApiEvent(webEvent);

            return apiEvent;
        }
    }
}