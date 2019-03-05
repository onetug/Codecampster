using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "sponsor")]
    public class ApiSponsor
    {
        // For Json.Net Serialization/Deserialization
        public ApiSponsor()
        {

        }

        public ApiSponsor(Sponsor webSponsor, Uri imageUrl = null,
            bool includeDetails = false)
        {
            if (webSponsor == null)
                return;

            Id = webSponsor.SponsorId;

            // Summary

            EventId = webSponsor.EventId;

            CompanyName = webSponsor.CompanyName;
            ImageUrl = imageUrl;

            SponsorLevel = webSponsor.SponsorLevel;

            if (!includeDetails)
                return;

            // Details

            Bio = webSponsor.Bio;
            WebsiteUrl = webSponsor.WebsiteUrl;
            TwitterHandle = webSponsor.TwitterHandle;
            EmailAddress = webSponsor.EmailAddress;
        }

        public int Id { get; set; }

        #region Summary

        public int? EventId { get; set; }

        public string CompanyName { get; set; }

        public Uri ImageUrl { get; set; }

        public int SponsorLevel { get; set; }

        #endregion

        #region Details

        public string Bio { get; set; }

        public string WebsiteUrl { get; set; }

        public string TwitterHandle { get; set; }

        public string EmailAddress { get; set; }

        #endregion

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - Level {SponsorLevel} - {CompanyName}";
    }
}
