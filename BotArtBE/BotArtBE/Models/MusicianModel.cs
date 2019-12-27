using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotArtBE.Models
{
    public class MusicianModel : TableEntity
    {
        public string Name { get; set; }
        public string Properties { get; set; }
        public int Votes { get; set; }
    }

    public class MusicianNetModel 
    {
        public string Name { get; set; }
        public MusicianProperties Properties { get; set; }
        public int Votes { get; set; }
        public string Submitter { get; set; }
    }

}
