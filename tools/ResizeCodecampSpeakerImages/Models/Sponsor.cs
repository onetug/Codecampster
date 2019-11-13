using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Sponsor
    {
        public int SponsorId { get; set; }

        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Sponsorship Level")]
        public int SponsorLevel { get; set; }

        public string Bio { get; set; }

        [Display(Name = "Twitter")]
        public string TwitterHandle { get; set; }

        [Display(Name ="Website")]
        public string WebsiteUrl { get; set; }

        public byte[] Image { get; set; }

        [Display(Name = "Point Of Contact")]
        public string PointOfContact { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public Event Event { get; set; }
    }
}
