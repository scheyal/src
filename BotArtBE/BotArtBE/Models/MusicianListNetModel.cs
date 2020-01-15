using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotArtBE.Models
{
    public class MusicianVotes
    {
        public string Name { get; set; }
        public int Votes { get; set; }

        public MusicianVotes(string name, int votes)
        {
            Name = name;
            Votes = votes;
        }
    }
    public class MusicianListNetModel
    {
        public List<MusicianVotes> Musicians { get; set; }

        public MusicianListNetModel()
        {
            Musicians = new List<MusicianVotes>();
        }
    }
}
