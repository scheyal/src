namespace Activity3PService.Models
{
    public static class Utilities
    {
        const long MaxQuantity = 10000;

        public static Random RandGen;


        static Utilities()
        {
            RandGen = new Random();
        }

        public static long GetQuantity(long timeTicks, long Max = Utilities.MaxQuantity, long Min = 0)
        {
            double rads = Math.PI / 180.0 * (double)(timeTicks % 180);
            long value = (long)((double)Max * Math.Sin(rads)) + Max / 2;
            value += value < Min ? Min : 0;
            return value;
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

        public static string GetRandomString(string[] stringArray)
        {
            int r = RandGen.Next(stringArray.Length);
            return stringArray[r];
        }

        public static double GetRandomFraction()
        {
            return (double)RandGen.NextDouble();
        }

    }
}
