using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "announcement")]
    public class ApiAnnouncement
    {
        // For Json.Net Serialization/Deserialization
        public ApiAnnouncement()
        {

        }

        public ApiAnnouncement(Announcement webAnnouncement)
        {
            Id = webAnnouncement.AnnouncementId;

            // Summary

            EventId = webAnnouncement.EventId;

            Rank = webAnnouncement.Rank;

            PublishOn = webAnnouncement.PublishOn;
            ExpiresOn = webAnnouncement.ExpiresOn;

            // Details

            Message = webAnnouncement.Message;
        }

        public int Id { get; set; }

        #region Summary

        public int? EventId { get; set; }

        public int Rank { get; set; }

        public DateTime PublishOn { get; set; }

        public DateTime? ExpiresOn { get; set; }

        #endregion

        #region Details

        public string Message { get; set; }

        #endregion

        private string DebuggerDisplay =>
            $"{Id} - Event {EventId} - Rank {Rank}" +
            $" - Start {PublishOn:g} - End {ExpiresOn:g} - {Message,-20}";
    }
}