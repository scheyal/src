using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.SwaggerGen;
using System;
using System.IO.Pipelines;
using System.Text;
using System.Text.Json;
using Activity3PService.Models;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Activity3PService.Controllers
{
    // [Route("api/[controller]")]
    [Route("[controller]/[action]")]
    [ApiController]
    public class LogonController : ControllerBase
    {

        private readonly ILogger<LogonController> _logger;
        private readonly IConfiguration _configuration;

        public LogonController(ILogger<LogonController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        // POST api/<LogonController>
        [HttpPost]
        public async Task<IActionResult> GetToken([FromBody] LogonModel creds)
        {

            string RequestBody = await GetRawBodyAsync();

            _logger.LogInformation(RequestBody);

            string u = creds.Username;
            string p = creds.Password;
            if(!creds.Authenticate())
            {
                return Unauthorized("Bad Credentials");
            }          

            return Ok(creds.Token());

        }
        
        [HttpGet(Name = "Verify")]
        // [HttpGet("{token}")]
        public IActionResult Verify(string token)
        {
            bool ok = new LogonModel().VerifyToken(token);
            return ok ? Ok("Welcome") : Unauthorized("Bad Credentials");
        }

        ///
        /// Private helpers
        /// 

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
