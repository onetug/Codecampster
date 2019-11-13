using Codecamp.Data;
using Codecamp.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface IAnnouncementsApiBusinessLogic
    {
        List<ApiAnnouncement> GetAnnouncementsList(int? eventId);
    }

    public class AnnouncementsApiBusinessLogic
        : ApiBusinessLogic, IAnnouncementsApiBusinessLogic
    {
        public AnnouncementsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiAnnouncement> GetAnnouncementsList(int? eventId = null)
        {
            var apiAnnouncementsList = Context.Announcements
                .Where(announcement => announcement.EventId == eventId || eventId == null)
                .OrderByDescending(announcement => announcement.AnnouncementId)
                .Select(announcement => new ApiAnnouncement(announcement))
                .ToList();

            return apiAnnouncementsList;
        }
    }
}