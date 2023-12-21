using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace SqlActSim
{

    static class GlobalStatic
    {
        static public long Counter { get; set; } = 0;
        static public long TotalActivities { get; set; } = 0;
        static public string Watermark { get; set; } = "SqlActSim";

        static public DateTime ConsumptionStartDate { get; set; } = DateTime.Parse("01/01/2020");
        static public DateTime ConsumptionEndDate { get; set; } =  DateTime.Now.AddDays(-10);

        static public long PostedTransactionDelay = 5;

        // -- Methods --

        static GlobalStatic()
        {
        }

    }
}
