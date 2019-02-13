using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject(Title = "session")]
    public class ApiSession
    {
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

        public int Id { get; }

        #region Summary

        public int? EventId { get; }

        public string Name { get; }

        // TODO Multiple speakers
        public int? SpeakerId { get; }

        public bool IsApproved { get;  }

        public int? TrackId { get;  }

        public int? TimeslotId { get;  }

        public int SkillLevel { get;  }

        public string Keywords { get;  }

        #endregion

        #region Details

        public string Description { get;  }

        #endregion

        private string DebuggerDisplay => $"Session {Id}" + DebugSpeakerId + $" - {Name}";

        private string DebugSpeakerId =>
            SpeakerId == null ? "" : $" - Speaker {SpeakerId}";
    }
}
