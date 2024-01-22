using Activity3PService.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Text;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Activity3PService.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class ActivityController : ControllerBase
    {
        IConfiguration _configuration;
        private readonly ILogger<ActivityController> _logger;


        public ActivityController(ILogger<ActivityController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /*** DELETE LATER ***
        // GET: api/<ActivityController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "TEST Only 1", "TEST Only 2" };
        }

        // GET api/<ActivityController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return $"TEST Only: {id}";
        }
        ***/

        /// <summary>
        /// Generate Purcahsed Electricity activities from WatermarkDate to today with activity per day with specified attributes (Facility, OU, Name seed).
        /// </summary>
        /// <param name="Facility">Facility for the generated activities</param>
        /// <param name="OU">Organizational Unit for the generated activites</param>
        /// <param name="WatermarkDate">Date from which the activities are generated, one activity per day</param>
        /// <param name="NameSeed">A seed for the name/description fields</param>
        /// <param name="ActivityType">Hard-coded Purchased Electricity. Use other for API error</param>
        /// <param name="Fail">Generate failed records for error testing (with some invalid fields)</param>
        /// <returns></returns>
        [HttpGet(Name = "GetActivities")]
        public async Task<ActivityResponseModel> GetActivities(string Facility, string OU, string WatermarkDate = "01/01/2024", string NameSeed = "Test 3P Utilities", string ActivityType = Globals.PurchasedElectricity, bool Fail=false)
        {

            ActivityResponseModel activityResponseModel = new ActivityResponseModel();

            try
            {
                // Verify Authentication
                LogonModel auth = new LogonModel();
                string? authHeader = HttpContext.Request.Headers["Authorization"];
                string token = auth.ExtractHeaderToken(authHeader);
                bool Allow = auth.VerifyToken(token);

                if (!Allow)
                {
                    activityResponseModel.SetStatus(StatusCodes.Status401Unauthorized, "Unauthorized");
                    return activityResponseModel;
                }

                if (ActivityType != Globals.PurchasedElectricity)
                {
                    activityResponseModel.SetStatus(StatusCodes.Status405MethodNotAllowed, $"Incorrect Activity Type {ActivityType}. Only '{Globals.PurchasedElectricity}' type is supported now.");
                    return activityResponseModel;
                }

                DateTime Since = DateTime.Parse(WatermarkDate);
                if(DateTime.Now < Since) 
                {
                    throw new Exception($"Invalid WatermarkDate {WatermarkDate}");
                }

                // Prase input

                string RequestText = $"Params: AT={ActivityType};F={Facility}; OU={OU}; WD={WatermarkDate} -- Body: <" + await GetRawBodyAsync() + ">";
                _logger.LogInformation(RequestText);


                // Generate response

                List<ActivityModel> activities = new List<ActivityModel>();
                int c = 0;
                foreach (DateTime day in EachDay(Since, DateTime.Now))
                {
                    ActivityModel act = new ActivityModel(NameSeed, day, Facility, OU, ++c, Fail);
                    activities.Add(act);
                }

                activityResponseModel.SetActivities(activities);
                activityResponseModel.SetStatus(StatusCodes.Status200OK, "OK");

            }
            catch (Exception ex)
            {
                activityResponseModel.SetStatus(500, $"Internal Server Error: {ex.Message}");
            }

            return activityResponseModel;

        }


        ///
        /// Private helpers
        /// 

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

        async Task<string> GetRawBodyAsync()
        {
            var request = HttpContext.Request;
            if (!request.Body.CanSeek)
            {
                // We only do this if the stream isn't *already* seekable,
                // as EnableBuffering will create a new stream instance
                // each time it's called
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            var reader = new StreamReader(request.Body, Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            request.Body.Position = 0;

            return body;
        }
    }
}
