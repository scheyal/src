using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

namespace ArtistChatBot.Controllers
{
    [Route("api/bot")]
    [ApiController]
    public class MainController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private IBotFrameworkHttpAdapter _adapter;
        private IBot _bot;

        public MainController(IBotFrameworkHttpAdapter adapter, IBot bot)
        {
            _adapter = adapter;
            _bot = bot;
        }

        [HttpPost]
        public async Task PostAsync()
        {
            // Delegate the processing of the HTTP POST to the adapter.
            // The adapter will invoke the bot.
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
    }
}