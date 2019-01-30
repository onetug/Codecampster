using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Models.Api;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.BusinessLogic.Api
{
    public interface ISpeakersApiBusinessLogic
    {
        Task<List<ApiSpeaker>> GetSpeakersList();

        Task<ApiSpeaker> GetApiSpeaker(int speakerId,
            bool includeDetails = false);

        Task<Speaker> GetWebSpeaker(int speakerId);
    }

    public class SpeakersApiBusinessLogic : ApiBusinessLogic, ISpeakersApiBusinessLogic
    {
        public SpeakersApiBusinessLogic(CodecampDbContext context) : base(context,
            "speakers")
        {
        }

        public async Task<List<ApiSpeaker>> GetSpeakersList()
        {
            var apiSpeakerList =
                await Context.Speakers
                    .OrderBy(speaker => speaker.SpeakerId)
                    .Include(speaker => speaker.CodecampUser)
                    .Select(speaker =>
                        new ApiSpeaker(speaker,
                            GetImageUrl(speaker.SpeakerId),
                            false))
                    .ToListAsync();

            return apiSpeakerList;
        }

        public async Task<ApiSpeaker> GetApiSpeaker(int speakerId,
            bool includeDetails = false)
        {
            var webSpeaker = await Context.Speakers
                .Include(speaker => speaker.CodecampUser)
                .FirstOrDefaultAsync(speaker => speaker.SpeakerId == speakerId);

            if (webSpeaker == null)
                return null;

            var apiSpeaker =
                new ApiSpeaker(webSpeaker, GetImageUrl(speakerId), includeDetails);

            return apiSpeaker;
        }

        public async Task<Speaker> GetWebSpeaker(int speakerId)
        {
            var webSpeaker = await Context.Speakers.FindAsync(speakerId);

            return webSpeaker;
        }
    }
}
