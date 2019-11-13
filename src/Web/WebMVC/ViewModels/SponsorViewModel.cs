using Codecamp.BusinessLogic;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public class SponsorViewModel
    {
        // Lets set the max file size to 10 MB, that is way big enough
        public const int MaxImageSize = 10000000;

        public int SponsorId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [Display(Name = "Sponsorship Level")]
        public int SponsorLevel { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Twitter")]
        public string TwitterHandle { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Website")]
        public string WebsiteUrl { get; set; }

        [ImageSizeValidation(MaxImageSize)]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [Display(Name = "Point Of Contact")]
        public string PointOfContact { get; set; }

        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Event Name")]
        public string EventName { get; set; }
    }
}
