using Newtonsoft.Json;

namespace Codecamp.Models.Api
{
    [JsonObject(Title = "sponsorLevel")]
    public class ApiSponsorLevel
    {
        public int SponsorLevelId { get; set; }

        public string SponsorLevelName { get; set; }
    }
}
