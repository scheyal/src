using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BFBulletin.Api
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class BulletinController : ControllerBase
    {

        private IConfiguration AppConfig;

        public BulletinController(IConfiguration configuration)
        {
            AppConfig = configuration;
        }

        // GET: api/Bulletin
        [HttpGet]
        public async Task<ActionResult<Models.Bulletin>> Get()
        {
            Models.Bulletin Bulletin = new Models.Bulletin();
            try
            {
                Models.BulletinMgr mgr = new Models.BulletinMgr(AppConfig);
                Bulletin = await mgr.GetBulletin();
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.TraceError($"Error: Cannot find bulletin. Exception: {e.Message}");
            }
            return Bulletin;
        }

        // GET: api/Bulletin/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        /*** BUGBUG: Maybe something in the future with auth/z etc. Nothing now.
        // POST: api/Bulletin
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Bulletin/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        ***/
    }
}
