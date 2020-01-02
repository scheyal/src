using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ArtistChatBot.Pages
{
    public class IndexModel : PageModel
    {
        public string BotUrl { get; set; }
        public void OnGet()
        {
            BotUrl = $"{Request.Scheme}://{Request.Host}/api/bot";

        }
    }
}
