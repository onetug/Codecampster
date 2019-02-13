using Codecamp.Data;
using Codecamp.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface ITimeslotsApiBusinessLogic
    {
        List<ApiTimeslot> GetTimeslotsList(int? eventId);
    }

    public class TimeslotsApiBusinessLogic : ApiBusinessLogic, ITimeslotsApiBusinessLogic
    {
        public TimeslotsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiTimeslot> GetTimeslotsList(int? eventId = null)
        {
            var apiTimeslotsList = Context.Timeslots
                .Where(timeslot => timeslot.EventId == eventId || eventId == null)
                .OrderBy(timeslot => timeslot.TimeslotId)
                .Select(timeslot => new ApiTimeslot(timeslot))
                .ToList();

            return apiTimeslotsList;
        }
    }
}