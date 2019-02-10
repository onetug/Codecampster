using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject("speaker")]
    public class ApiSpeaker
    {
        public ApiSpeaker(Speaker webSpeaker, Uri imageUrl = null,
            bool includeDetails = false)
        {
            if (webSpeaker == null)
                return;

            Id = webSpeaker.SpeakerId;

            ImageUrl = imageUrl;
            User = (webSpeaker.CodecampUser == null)
                ? null
                : new ApiUser(webSpeaker.CodecampUser, includeDetails);

            IsMvp = webSpeaker.IsMvp;
            IsApproved = webSpeaker.IsApproved;

            // TODO Future
            //EventId = webSpeaker.EventId;

            if (!includeDetails)
                return;

            CompanyName = webSpeaker.CompanyName;
            Bio = webSpeaker.Bio;
            WebsiteUrl = webSpeaker.WebsiteUrl;
            BlogUrl = webSpeaker.BlogUrl;
            LinkedIn = webSpeaker.LinkedIn;
        }

        #region Summary

        public int Id { get; }

        [JsonProperty("user")]
        public ApiUser User { get; }

        public string Name => User?.FullNameOrEmailAddress;

        public Uri ImageUrl { get; }

        public bool IsMvp { get; }

        public bool IsApproved { get; }

        // TODO Future
        //public int? EventId { get; }

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
