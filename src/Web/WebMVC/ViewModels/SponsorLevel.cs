using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codecamp.ViewModels
{
    public enum LevelsOfSponsorship
    {
        Coffee = 1,
        Breakfast = 2,
        AttendeePartySponsor = 3,
        SpeakerPartySponsor = 4,
        LunchSponsor = 5,
        Bronze = 6,
        Silver = 7,
        Gold = 8,
        Platinum = 9
    }

    public class SponsorLevel
    {
        public static string Coffee = "Coffee Level Sponsor";
        public static string Breakfast = "Breakfast Level Sponsor";
        public static string AttendeePartySponsor = "Attendee Party Level Sponsor";
        public static string SpeakerPartySponsor = "Speaker Party Level Sponsor";
        public static string LunchSponsor = "Lunch Level Sponsor";
        public static string Bronze = "Bronze Level Sponsor";
        public static string Silver = "Silver Level Sponsor";
        public static string Gold = "Gold Level Sponsor";
        public static string Platinum = "Platinum Level Sponsor";

        public static IList<SponsorLevel> GetSponsorshipLevels()
        {
            var sponsorshipLevels = new List<SponsorLevel>();

            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Coffee, Description = Coffee });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Breakfast, Description = Breakfast });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.AttendeePartySponsor, Description = AttendeePartySponsor });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.SpeakerPartySponsor, Description = SpeakerPartySponsor });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.LunchSponsor, Description = LunchSponsor });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Bronze, Description = Bronze });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Silver, Description = Silver });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Gold, Description = Gold });
            sponsorshipLevels.Add(
                new SponsorLevel { SponsorshipLevelId = (int)LevelsOfSponsorship.Platinum, Description = Platinum });

            return sponsorshipLevels;
        }

        public static int GetSponsorshipDisplayLevel(int sponsorshipLevel)
        {
            switch (sponsorshipLevel)
            {
                case (int)LevelsOfSponsorship.Platinum:
                    return 1;
                case (int)LevelsOfSponsorship.Gold:
                    return 2;
                case (int)LevelsOfSponsorship.AttendeePartySponsor:
                case (int)LevelsOfSponsorship.SpeakerPartySponsor:
                case (int)LevelsOfSponsorship.LunchSponsor:
                    return 3;
                case (int)LevelsOfSponsorship.Silver:
                    return 4;
                case (int)LevelsOfSponsorship.Coffee:
                case (int)LevelsOfSponsorship.Breakfast:
                case (int)LevelsOfSponsorship.Bronze:
                default:
                    return 5;
            }

        }

        public static string GetSponsorshipLevelDescription(int sponsorshipLevel)
        {
            switch (sponsorshipLevel)
            {
                case (int)LevelsOfSponsorship.Coffee:
                    return Coffee;
                case (int)LevelsOfSponsorship.Breakfast:
                    return Breakfast;
                case (int)LevelsOfSponsorship.AttendeePartySponsor:
                    return AttendeePartySponsor;
                case (int)LevelsOfSponsorship.SpeakerPartySponsor:
                    return SpeakerPartySponsor;
                case (int)LevelsOfSponsorship.LunchSponsor:
                    return LunchSponsor;
                case (int)LevelsOfSponsorship.Bronze:
                    return Bronze;
                case (int)LevelsOfSponsorship.Silver:
                    return Silver;
                case (int)LevelsOfSponsorship.Gold:
                    return Gold;
                case (int)LevelsOfSponsorship.Platinum:
                    return Platinum;
                default:
                    return "Not specified";
            }
        }

        public int SponsorshipLevelId { get; set; }
        public string Description { get; set; }
    }
}
