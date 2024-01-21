using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Http;

namespace Activity3PService.Models
{
    public class ActivityResponseModel
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public int Count { get; set; }
        public IEnumerable<ActivityModel> Activities { get; set; }

        public ActivityResponseModel(int status = StatusCodes.Status204NoContent, string message = "No Content", IEnumerable<ActivityModel>? activities = null) 
        {
            StatusCode = status;
            Message = message;
            Count = 0;
            Activities = activities ?? new List<ActivityModel>();
        }

        public void SetStatus(int status, string msg) { StatusCode = status ; Message = msg;}

        public void SetActivities(IEnumerable<ActivityModel> activities) 
        {
            Activities = activities ?? new List<ActivityModel>();
            Count = Activities.Count();
        }

    }
}
