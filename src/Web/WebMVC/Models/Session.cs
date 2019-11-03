using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Codecamp.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Session
    {
        public int SessionId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Skill Level")]
        public int SkillLevel { get; set; }

        // Keywords associated with the session.
        public string Keywords { get; set; }

        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        // The sessions associated with speakers
        public virtual List<SpeakerSession> SpeakerSessions { get; set; }

        // The sessions users have favorited
        public virtual List<AttendeeSession> AttendeeSessions { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public virtual Event Event { get; set; }

        [ForeignKey("Track Id")]
        public int? TrackId { get; set; }

        public virtual Track Track { get; set; }

        [ForeignKey("Timeslot Id")]
        public int? TimeslotId { get; set; }

        public virtual Timeslot Timeslot { get; set; }

        private string DebuggerDisplay =>
            $@"{SessionId} - Time {TimeslotId?.ToString() ?? "?"} - " + 
            $@"Track {TrackId?.ToString() ?? "?"} - {Name}";
    }
}
