using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public string Name { get; set; }

        [Display(Name = "Social Media Hashtag")]
        public string SocialMediaHashtag { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Start Date")]
        public DateTime StartDateTime { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "End Date")]
        public DateTime EndDateTime { get; set; }

        [Display(Name = "Location")]
        public string LocationAddress { get; set; }

        /// <summary>
        /// Is this the currently active event
        /// </summary>
        [Display(Name = "Is the Active Event")]
        public bool IsActive { get; set; }

        [Display(Name = "Attendee Registration is Open")]
        public bool IsAttendeeRegistrationOpen { get; set; }

        [Display(Name = "Speaker Registration is Open")]
        public bool IsSpeakerRegistrationOpen { get; set; }

        /// <summary>
        /// List of sessions for the event
        /// </summary>
        public List<Session> Sessions { get; set; }
        
        /// <summary>
        /// List of speakers for the event
        /// </summary>
        public List<Speaker> Speakers { get; set; }
    }
}
