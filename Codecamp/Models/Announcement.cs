using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace Codecamp.Models
{
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public class Announcement
    {
        public int AnnouncementId { get; set; }

        public int? EventId { get; set; }

        public string Message { get; set; }

        [Display(Name = "Display Order")]
        public int Rank { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Publish On")]
        public DateTime PublishOn { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Expires On")]
        public DateTime? ExpiresOn { get; set; }

        private string DebuggerDisplay =>
            $"{AnnouncementId} - Event {EventId} - Rank {Rank} - " +
            $"Publish {PublishOn:g} - Expire {ExpiresOn:g} - {Message,-20}";
    }
}
