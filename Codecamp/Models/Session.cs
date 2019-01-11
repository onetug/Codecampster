using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Session
    {
        public int SessionId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        [Display(Name = "Skill Level")]
        public int SkillLevel { get; set; }

        // Keywords associated with the session.
        public string Keywords { get; set; }

        public bool IsApproved { get; set; }

        // The sessions associated with speakers
        public virtual List<SpeakerSession> SpeakerSessions { get; set; }

        // The sessions users have favorited
        public virtual List<AttendeeSession> AttendeeSessions { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public virtual Event Event { get; set; }

        [ForeignKey("TrackId")]
        public int? TrackId { get; set; }

        public virtual Track Track { get; set; }

        [ForeignKey("TimeslotId")]
        public int? TimeslotId { get; set; }

        public virtual Timeslot Timeslot { get; set; }
    }
}
