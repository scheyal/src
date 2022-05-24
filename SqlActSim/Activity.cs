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
        public void FillTestLine();
        public void PrintTestLine();
        public void GenerateTestLines(int count);


    }
}
