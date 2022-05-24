using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlActSim
{
    static class GlobalStatic
    {
        static public long Counter { get; set; }
        static public string Watermark { get; set; }


        static GlobalStatic()
        {
            Counter = 0;
            Watermark = "SqlActSim";


        }
    }
}
