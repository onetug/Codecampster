using Codecamp.Data;
using Codecamp.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface IEventsApiBusinessLogic
    {
        List<ApiEvent> GetEventsList();

        ApiEvent GetActiveEvent();

        ApiEvent GetEvent(int year);
    }

    public class EventsApiBusinessLogic : ApiBusinessLogic, IEventsApiBusinessLogic
    {
        public EventsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiEvent> GetEventsList()
        {
            var apiEventsList = Context.Events
                .Select(e => new ApiEvent(e))
                .ToList();

            return apiEventsList;
        }
    }
}