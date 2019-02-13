using Newtonsoft.Json;
using System;
using System.Diagnostics;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "event")]
    public class ApiEvent
    {
        public ApiEvent(Event webEvent)
        {
            Id = webEvent.EventId;

            // Summary
            Name = webEvent.Name;
            StartDateTime = webEvent.StartDateTime;
            EndDateTime = webEvent.EndDateTime;

            // State
            IsActive = webEvent.IsActive;
            IsSpeakerRegistrationOpen = webEvent.IsSpeakerRegistrationOpen;
            IsAttendeeRegistrationOpen = webEvent.IsAttendeeRegistrationOpen;

            // Details
            LocationAddress = webEvent.LocationAddress;
            SocialMediaHashtag = webEvent.SocialMediaHashtag;
        }

        #region Summary

        public int Id { get; set; }

        public string Name { get; set; }

        public DateTime StartDateTime { get; set; }

        public DateTime EndDateTime { get; set; }

        #endregion

        #region State

        public bool IsActive { get; set; }

        public bool IsAttendeeRegistrationOpen { get; set; }

        public bool IsSpeakerRegistrationOpen { get; set; }

        #endregion

        #region Details

        public string SocialMediaHashtag { get; set; }

        public string LocationAddress { get; set; }

        #endregion

        private string DebuggerDisplay => $"{Name} - {StartDateTime:g} to {EndDateTime:g}";
    }
}
