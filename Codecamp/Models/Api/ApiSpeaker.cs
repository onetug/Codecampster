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

        public int Id { get; }

        #region Summary

        public int? EventId { get; }

        public string Name => User?.FullNameOrEmailAddress;

        [JsonProperty("user")]
        public ApiUser User { get; }

        public Uri ImageUrl { get; }

        public bool IsMvp { get; }

        public bool IsApproved { get; }

        #endregion

        #region Details

        public string CompanyName { get; }

        public string Bio { get; }

        public string WebsiteUrl { get; }

        public string BlogUrl { get; }

        public string LinkedIn { get; }

        #endregion

        private string DebuggerDisplay => $"{Id} - {Name}";
    }
}
