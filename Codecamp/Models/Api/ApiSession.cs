using Newtonsoft.Json;
using System.Diagnostics;
using System.Linq;

namespace Codecamp.Models.Api
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    [JsonObject("session")]
    public class ApiSession
    {
        public ApiSession(Session webSession, bool includeDescription = false)
        {
            Id = webSession.SessionId;

            Name = webSession.Name;
            SpeakerId = webSession.SpeakerSessions?.FirstOrDefault()?.SpeakerId;;

            IsApproved = webSession.IsApproved;
            TrackId = webSession.TrackId;
            TimeslotId = webSession.TimeslotId;

            SkillLevel = webSession.SkillLevel;
            Keywords = webSession.Keywords;

            if (!includeDescription)
                return;

            Description = webSession.Description;
        }

        #region Summary

        public int Id { get; }

        public string Name { get; }

        // TODO Multiple speakers
        public int? SpeakerId { get; }

        // TODO Future
        //public int? EventId { get; }

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
