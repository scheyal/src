using Activity3PService.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Activity3PService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET api/<ActivityController>
        [HttpGet]
        public ActivityResponseModel Get()
        {
            return new ActivityResponseModel(StatusCodes.Status200OK, "Test OK!");
        }
    }
}
