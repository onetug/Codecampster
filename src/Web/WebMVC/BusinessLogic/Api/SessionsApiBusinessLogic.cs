using Codecamp.Data;
using Codecamp.Models.Api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface ISessionsApiBusinessLogic
    {
        List<ApiSession> GetSessionsList(int? eventId,
            int? trackId,
            int? timeslotId,
            bool includeDescriptions);

        ApiSession GetSession(int sessionId,
            bool includeDescription);
    }

    public class SessionsApiBusinessLogic : ApiBusinessLogic, ISessionsApiBusinessLogic
    {
        public SessionsApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiSession> GetSessionsList(int? eventId = null,
            int? trackId = null,
            int? timeslotId = null,
            bool includeDescriptions = false)
        {
            var apiSessionList = Context.Sessions
                .Where(session => session.EventId == eventId || eventId == null )
                .Where(session => session.TrackId == trackId || trackId == null )
                .Where(session => session.TimeslotId == timeslotId || timeslotId == null)
                .OrderBy(session => session.SessionId)
                .Include(session => session.SpeakerSessions)
                .Select(session => new ApiSession(session, includeDescriptions))
                .ToList();

            return apiSessionList;
        }

        public ApiSession GetSession(int sessionId,
            bool includeDescription)
        {
            var webSession = Context.Sessions
                .OrderBy(session => session.SessionId)
                .Include(session => session.SpeakerSessions)
                .FirstOrDefault(session => session.SessionId == sessionId);

            if (webSession == null)
                return null;

            var apiSession = new ApiSession(webSession, includeDescription);

            return apiSession;
        }
    }
}
