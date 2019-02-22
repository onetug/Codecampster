using Codecamp.BusinessLogic;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using static Codecamp.Controllers.SpeakersController;

namespace Codecamp.ViewModels
{
    public class SpeakerViewModel
    {
        // Lets set the max file size to 5 MB, that is way big enough
        public const int MaxImageSize = 5000000;

        public int SpeakerId { get; set; }

        public string CodecampUserId { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Name")]
        public string FullName { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Company")]
        public string CompanyName { get; set; }

        [ImageSizeValidation(MaxImageSize)]
        [Display(Name = "Image")]
        public IFormFile ImageFile { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Bio")]
        public string Bio { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Website URL")]
        public string WebsiteUrl { get; set; }

        [DataType(DataType.Url)]
        [Display(Name = "Blog URL")]
        public string BlogUrl { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Location")]
        public string GeographicLocation { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Twitter")]
        public string TwitterHandle { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "LinkedIn")]
        public string LinkedIn { get; set; }

        [Required]
        [Display(Name = "Is Volunteering")]
        public bool IsVolunteer { get; set; }

        [Display(Name = "Is an MVP")]
        public bool IsMvp { get; set; }

        [DataType(DataType.Text)]
        [Display(Name = "Note to Organizers")]
        public string NoteToOrganizers { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; set; }
        
        [Required]
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }

        [Display(Name = "Sessions")]
        public List<SessionViewModel> Sessions { get; set; }
    }
}
