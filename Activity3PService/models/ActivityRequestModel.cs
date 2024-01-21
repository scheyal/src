namespace Activity3PService.Models
{
    public class ActivityRequestModel
    {
        public string ActivityType { get; set; }
        public DateTime SinceDate { get; set; }

        public ActivityRequestModel()
        {
            ActivityType = "Invalid";
            SinceDate = new DateTime(1900, 1, 1);
        }
    }
}
