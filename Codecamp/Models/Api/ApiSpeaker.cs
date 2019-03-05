using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    // TODO See Models.Speaker for additional fields
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "speaker")]
    public class ApiSpeaker
    {
        // For Json.Net Serialization/Deserialization
        public ApiSpeaker()
        {

        }

        public ApiSpeaker(Speaker webSpeaker, Uri imageUrl = null,
            bool includeDetails = false)
        {
            if (webSpeaker == null)
                return;

            Id = webSpeaker.SpeakerId;

            // Summary

            EventId = webSpeaker.EventId;

            User = (webSpeaker.CodecampUser == null)
                ? null
                : new ApiUser(webSpeaker.CodecampUser, includeDetails);
            ImageUrl = imageUrl;

            IsMvp = webSpeaker.IsMvp;
            IsApproved = webSpeaker.IsApproved;

            if (!includeDetails)
                return;

            // Details

            CompanyName = webSpeaker.CompanyName;
            Bio = webSpeaker.Bio;
            WebsiteUrl = webSpeaker.WebsiteUrl;
            BlogUrl = webSpeaker.BlogUrl;
            LinkedIn = webSpeaker.LinkedIn;
        }

        public int Id { get; set; }

        #region Summary

        public int? EventId { get; set; }

        public string Name => User?.FullNameOrEmailAddress;

        [JsonProperty("user")]
        public ApiUser User { get; set; }

        public Uri ImageUrl { get; set; }

        public bool IsMvp { get; set; }

        public bool IsApproved { get; set; }

        #endregion

        #region Details

        public string CompanyName { get; set; }

        public string Bio { get; set; }

        public string WebsiteUrl { get; set; }

        public string BlogUrl { get; set; }

        public string LinkedIn { get; set; }

        #endregion

        private string DebuggerDisplay => $"{Id} - {Name}";
    }
}
