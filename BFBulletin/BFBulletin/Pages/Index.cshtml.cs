using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace BFBulletin.Pages
{
    public class IndexModel : PageModel
    {
        public IConfiguration AppConfig { get; set; }

        public Models.Bulletin Bulletin { get; set; }

        public IndexModel(IConfiguration appconfig)
        {
            AppConfig = appconfig;
        }

        public async Task OnGet()
        {
            Models.BulletinMgr mgr = new Models.BulletinMgr(AppConfig);
            Bulletin = await mgr.GetBulletin();
        }

    }
}
