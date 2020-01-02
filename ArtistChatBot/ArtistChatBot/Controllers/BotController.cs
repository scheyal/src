// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;

/// <summary>
/// Just an example of how to use another controller. Both api/messages and api/bot (MainController) do the same exact thing.
/// </summary>

namespace ArtistChatBot
{
    [Route("api/messages")]
    [ApiController]
    public class BotController : ControllerBase
    {
        private IBotFrameworkHttpAdapter _adapter;
        private IBot _bot;

        public BotController(IBotFrameworkHttpAdapter adapter, IBot bot)
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
