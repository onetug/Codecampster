using Codecamp.Data;
using Codecamp.Models;
using Codecamp.Models.Api;
using Codecamp.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace Codecamp.BusinessLogic.Api
{
    public interface ISponsorsApiBusinessLogic
    {
        IList<SponsorLevel> GetSponsorLevels();

        IList<ApiSponsor> GetSponsorsList(int? eventId,
            bool includeDetails = false);

        Sponsor GetWebSponsor(int sponsorId);
    }

    public class SponsorsApiBusinessLogic : ApiBusinessLogic, ISponsorsApiBusinessLogic
    {
        public SponsorsApiBusinessLogic(CodecampDbContext context) : base(context,
            "sponsors")
        {
        }

        // TODO Sponsor levels are not currently tied to EventIds
        public IList<SponsorLevel> GetSponsorLevels()
        {
            var sponsorLevel = SponsorLevel.GetSponsorshipLevels();

            return sponsorLevel;
        }

        public IList<ApiSponsor> GetSponsorsList(int? eventId = null,
            bool includeDetails = false)
        {
            var apiSponsorsList = Context.Sponsors
                .Where(sponsor => sponsor.EventId == eventId || eventId == null)
                .OrderBy(sponsor => sponsor.SponsorId)
                .Select(sponsor =>
                    new ApiSponsor(sponsor,
                        GetImageUrl(sponsor.SponsorId),
                        includeDetails))
                .ToList();

            return apiSponsorsList;
        }

        public Sponsor GetWebSponsor(int sponsorId)
        {
            var webSponsor = Context.Sponsors.Find(sponsorId);

            return webSponsor;
        }
    }
}