using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Codecamp.Models
{
    // Add profile data for application users by adding properties to the CodecampUser class
    public class CodecampUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [PersonalData]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [PersonalData]
        [Display(Name = "Location")]
        public string GeographicLocation { get; set; }

        [PersonalData]
        [Display(Name = "Twitter")]
        public string TwitterHandle { get; set; }

        [Display(Name = "I am attending")]
        public bool IsAttending { get; set; }

        [Display(Name = "I want to volunteer")]
        public bool IsVolunteer { get; set; }

        [Display(Name = "I am a speaker")]
        public bool IsSpeaker { get; set; }

        [ForeignKey("Speaker")]
        public int? SpeakerId { get; set; }

        public Speaker Speaker { get; set; }

        [ForeignKey("Event")]
        public int? EventId { get; set; }

        public Event Event { get; set; }

        // List of sessions a user has favorited
        public virtual List<AttendeeSession> AttendeeSessions { get; set; }

        // Get method to generate the user's full name
        public string FullName
        {
            get
            {
                // No name(s) are specified, indicate so
                if (string.IsNullOrEmpty(FirstName) && string.IsNullOrEmpty(LastName)) return "No name specified";

                return FirstName + (FirstName.Length > 0 ? " " + LastName : LastName);
            }
        }
    }
}
