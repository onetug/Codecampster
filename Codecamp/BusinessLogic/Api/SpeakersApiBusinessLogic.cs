using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Models.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface ISpeakersApiBusinessLogic
    {
        List<ApiSpeaker> GetSpeakersList(int? eventId,
            bool includeDetails = false);

        ApiSpeaker GetApiSpeaker(int speakerId,
            bool includeDetails = false);

        Speaker GetWebSpeaker(int speakerId);
    }

    public class SpeakersApiBusinessLogic : ApiBusinessLogic, ISpeakersApiBusinessLogic
    {
        public SpeakersApiBusinessLogic(CodecampDbContext context) : base(context,
            "speakers")
        {
        }

        public List<ApiSpeaker> GetSpeakersList(int? eventId = null,
            bool includeDetails = false)
        {
            var apiSpeakerList = GetWebSpeakers()
                .Where(speaker => speaker.EventId == eventId || eventId == null )
                .Select(speaker =>
                    new ApiSpeaker(speaker,
                        GetImageUrl(speaker.SpeakerId),
                        includeDetails))
                .ToList();

            return apiSpeakerList;
        }

        public ApiSpeaker GetApiSpeaker(int speakerId,
            bool includeDetails = false)
        {
            var webSpeaker = GetWebSpeakers()
                .FirstOrDefault(speaker => speaker.SpeakerId == speakerId);

            if (webSpeaker == null)
                return null;

            var apiSpeaker =
                new ApiSpeaker(webSpeaker, GetImageUrl(speakerId), includeDetails);

            return apiSpeaker;
        }

        public Speaker GetWebSpeaker(int speakerId)
        {
            var webSpeaker = Context.Speakers.Find(speakerId);

            return webSpeaker;
        }

        private IIncludableQueryable<Speaker, CodecampUser> GetWebSpeakers()
        {
            var webSpeakersList = Context.Speakers
                .OrderBy(speaker => speaker.SpeakerId)
                .Include(speaker => speaker.CodecampUser);

            return webSpeakersList;
        }
    }
}
