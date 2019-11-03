using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
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
    }
}
