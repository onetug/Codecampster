using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Announcement
    {
        public int AnnouncementId { get; set; }
        public string Message { get; set; }
        public int Rank { get; set; }
        public DateTime PublishOn { get; set; }
        public DateTime? ExpiresOn { get; set; }
    }
}
