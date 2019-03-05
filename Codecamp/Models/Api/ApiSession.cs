using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "session")]
    public class ApiSession
    {
        // For Json.Net Serialization/Deserialization
        public ApiSession()
        {

        }

        public ApiSession(Session webSession, bool includeDescription = false)
        {
            if (webSession == null)
                return;

            Id = webSession.SessionId;

            // Summary

            EventId = webSession.EventId;

            Name = webSession.Name;
            SpeakerId = webSession.SpeakerSessions?.FirstOrDefault()?.SpeakerId;;

            IsApproved = webSession.IsApproved;
            TrackId = webSession.TrackId;
            TimeslotId = webSession.TimeslotId;

            SkillLevel = webSession.SkillLevel;
            Keywords = webSession.Keywords;

            if (!includeDescription)
                return;

            // Details

            Description = webSession.Description;
        }

        public int Id { get; set; }

        #region Summary

        public int? EventId { get; set; }

        public string Name { get; set; }

        // TODO Multiple speakers
        public int? SpeakerId { get; set; }

        public bool IsApproved { get; set; }

        public int? TrackId { get; set; }

        public int? TimeslotId { get; set; }

        public int SkillLevel { get; set; }

        public string Keywords { get; set; }

        #endregion

        #region Details

        public string Description { get; set; }

        #endregion

        private string DebuggerDisplay => $"Session {Id}" + DebugSpeakerId + $" - {Name}";

        private string DebugSpeakerId =>
            SpeakerId == null ? "" : $" - Speaker {SpeakerId}";
    }
}
