using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SurveyBE.Models;
using System.Text.Json;
using System.Globalization;

namespace SurveyBE.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class StoreController : ControllerBase
    {

        private IConfiguration AppConfig;

        public StoreController(IConfiguration configuration)
        {
            AppConfig = configuration;
        }

        private void Authenticate()
        {
            string AuthKey = AppConfig["AuthKey"];
            string ClientAuthKey = Request.Headers["AuthKey"];

            if (AuthKey != ClientAuthKey)
            {
                throw new UnauthorizedAccessException("Invalid Authentication Key");
            }

        }
        // POST: api/Store
        // e.g. https://localhost:44342/api/Store/Submit
        [HttpPost]
        [Route("Submit/")]
        public async Task<ActionResult> Post([FromBody] Object NetEntry)
        {
            Authenticate(); 
            try
            {

                System.Diagnostics.Trace.TraceInformation($"Received Post message.\n${NetEntry}");

                // ensure it is serializable to Json.
                string jsonString = JsonSerializer.Serialize(NetEntry);
                System.Diagnostics.Trace.TraceInformation($"Received Serialized:\n${jsonString}");

                SurveyEntryModel entry = JsonSerializer.Deserialize<SurveyEntryModel>(jsonString);

                // SurveyEntryModel entry = new SurveyEntryModel();
                // entry.Test = NetEntry.Test;

                SurveyMgr sm = new SurveyMgr(AppConfig);
                await sm.Create(entry);

            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Cannot record survey entry: {e.Message}");
                return new BadRequestResult();
            }

            return new OkResult();
        }

        [HttpGet]
        public string Get()
        {
            return "NoOp";
        }


    }
}
