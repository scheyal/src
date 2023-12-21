using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;

namespace SqlActSim
{
    internal interface IActivity
    {
        public string Name { get; set; }

        public SqlCommand CreateCommand(SqlConnection Connection);
        public void FillLine(long index);
        public void PrintLine();
        public void GenerateTestLines(int count)
        {
            for (int i = 0; i < count; i++)
            {
                FillLine(i);
                PrintLine();
            }
        }


    }
}
