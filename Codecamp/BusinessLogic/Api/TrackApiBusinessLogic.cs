using Codecamp.Data;
using Codecamp.Models.Api;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface ITracksApiBusinessLogic
    {
        List<ApiTrack> GetTracksList(int? eventId);
    }

    public class TracksApiBusinessLogic : ApiBusinessLogic, ITracksApiBusinessLogic
    {
        public TracksApiBusinessLogic(CodecampDbContext context) : base(context)
        {
        }

        public List<ApiTrack> GetTracksList(int? eventId = null)
        {
            var apiTracksList = Context.Tracks
                .Where(track => track.EventId == eventId || eventId == null)
                .OrderBy(track => track.TrackId)
                .Select(track => new ApiTrack(track))
                .ToList();

            return apiTracksList;
        }
    }
}