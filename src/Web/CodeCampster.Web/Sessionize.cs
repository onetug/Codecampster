namespace CodeCampster.Web
{
    public class Sessionize
    {
        //enclose any one of these urls in a script tag as follows
        // <script type="text/javascript" src="*url here*"></script>
        //for instance: <script type="text/javascript" src="https://sessionize.com/api/v2/qzwrc33z/view/GridSmart"></script>

        //schedule smart grid - best grid/table view, full mobile support - preferred
        public const string ScheduleSmartGridUrl = "https://sessionize.com/api/v2/qzwrc33z/view/GridSmart";
        //schedule table - old alternative for smart grid, works with regular schedules
        public const string ScheduleTableUrl = "https://sessionize.com/api/v2/qzwrc33z/view/GridTable";
        //schedule grid - old alternative for smart grid, works with irregular schedules, no mobile support (do not use unless necessary)
        public const string ScheduleUrl = "https://sessionize.com/api/v2/qzwrc33z/view/Grid";
        //sessions list
        public const string SessionsUrl = "https://sessionize.com/api/v2/qzwrc33z/view/Sessions";
        //speaker list - individual rows consisting of speaker info in expanded format
        public const string SpeakersUrl = "https://sessionize.com/api/v2/qzwrc33z/view/Speakers";
        //speaker wall - nice round images of speakers where you can click on an individual speaker to view popup details
        public const string SpeakersWallUrl = "https://sessionize.com/api/v2/qzwrc33z/view/SpeakerWall";
    }
}
