using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlActSim
{
    public static class Utilities
    {
        const long MaxQuantity = 10000;

        static Random RandGen;


        static Utilities()
        {
            RandGen = new Random();
        }

        public static long GetQuantity(long timeTicks, long Max = -1)
        {
            Max = Max == -1 ? MaxQuantity : Max;
            double rads = Math.PI / 180.0 * (double)(timeTicks % 180);
            return (long)((double)MaxQuantity * Math.Sin(rads)) + MaxQuantity / 2;
        }

        public static bool RandBool()
        {
            int r = RandGen.Next();
            return r > (int.MaxValue / 2) ? true : false;
        }

        public static int TriRand()
        {
            int r = RandGen.Next();
            return r < (int.MaxValue / 3) ? 1 : (r > (int.MaxValue / 3 * 2) ? 3 : 2);

        }
     
        public static string GetTriRand(string s1, string s2, string s3)
        {
            int r = TriRand();
            return r == 1 ? s1 : (r == 2 ? s2 : s3);

        }

    }
}
