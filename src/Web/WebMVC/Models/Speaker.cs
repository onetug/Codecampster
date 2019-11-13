using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace Codecamp.Models
{
    // TODO Add Twitter handle
    // TODO Add separate email address for attendees versus presentation (work vs home)
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Speaker
    {
        public int SpeakerId { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        public string Bio { get; set; }

        [Display(Name = "Website")]
        public string WebsiteUrl { get; set; }

        [Display(Name = "Blog")]
        public string BlogUrl { get; set; }

        public byte[] Image { get; set; }

        [Display(Name = "Note to Organizers")]
        public string NoteToOrganizers { get; set; }

        public bool IsMvp { get; set; }

        public string LinkedIn { get; set; }

        // The CodecampUser associated with the speaker
        [ForeignKey("CodecampUser")]
        public string CodecampUserId { get; set; }
        public virtual CodecampUser CodecampUser { get; set; }

        public bool IsApproved { get; set; }

        // List of sessions associated with the speaker
        public virtual List<SpeakerSession> SpeakerSessions { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public Event Event { get; set; }

        private string DebuggerDisplay =>
            $"{SpeakerId} - " +
            (CodecampUser?.FullName ?? CodecampUser?.Email ?? "User name not available");
    }
}
