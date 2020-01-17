using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BotArtBE.Models
{
    public class MusicianVotes: IEquatable<MusicianVotes>, IComparable<MusicianVotes>
    {
        public string Name { get; set; }
        public int Votes { get; set; }

        public MusicianVotes(string name, int votes)
        {
            Name = name;
            Votes = votes;
        }

        public bool Equals(MusicianVotes other)
        {
            if(other == null)
            {
                return false;
            }
            if(other.Votes == this.Votes)
            {
                return true;
            }
            return false;
        }

        public int CompareTo(MusicianVotes other)
        {
            return Compare(other, this);
        }

        public static int Compare(MusicianVotes x, MusicianVotes y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // lengths of the two strings.
                    //
                    int retVal = 0;
                    if(x.Votes > y.Votes)
                    {
                        retVal = 1;
                    }
                    else if (x.Votes < y.Votes)
                    {
                        retVal = -1;
                    }
                    // else they're equal
                    return retVal;
                }
            }
        }

        public void Sort(List<MusicianVotes> musicians)
        {
            musicians.Sort(MusicianVotes.Compare);
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
