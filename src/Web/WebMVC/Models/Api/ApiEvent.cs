using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "event")]
    public class ApiEvent
    {
        // For Json.Net Serialization/Deserialization
        public ApiEvent()
        {

        }

        public ApiEvent(Event webEvent)
        {
            if (webEvent == null)
                return;

            Id = webEvent.EventId;

            // Summary

            Name = webEvent.Name;
            StartDateTime = webEvent.StartDateTime;
            EndDateTime = webEvent.EndDateTime;

            IsActive = webEvent.IsActive;
            IsSpeakerRegistrationOpen = webEvent.IsSpeakerRegistrationOpen;
            IsAttendeeRegistrationOpen = webEvent.IsAttendeeRegistrationOpen;

            // Details

            LocationAddress = webEvent.LocationAddress;
            SocialMediaHashtag = webEvent.SocialMediaHashtag;
        }

        public int Id { get; set; }

        #region Summary

        public string Name { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        public bool IsActive { get; set; }

        public bool IsAttendeeRegistrationOpen { get; set; }

        public bool IsSpeakerRegistrationOpen { get; set; }

        #endregion

        #region Details

        public string SocialMediaHashtag { get; set; }

        public string LocationAddress { get; set; }

        #endregion

        private string DebuggerDisplay =>
            $"{Id} - {Name} - {StartDateTime:g} to {EndDateTime:g}";
    }
}
