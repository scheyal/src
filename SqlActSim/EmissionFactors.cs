using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace SqlActSim
{
    /// <summary>
    /// Table of emission factors with RD (type, subtype) pivots
    /// Format: Name,Type,SubType,Library
    /// </summary>
    internal class EmissionFactors
    {
        const string EFCSVMapFilePath = ".\\MSMEF.csv";
        DataTable Table;
        enum ColumnIDs
        {
            Name = 0,
            Type = 1,
            SubType = 2,
            Library = 3,
            Count = 4
        }


        public void LoadFactors()
        {
            Table = Load(EFCSVMapFilePath);
        }
        private DataTable Load(string filePath)
        {
            DataTable tbl = new DataTable();

            tbl.Columns.Add(new DataColumn("Name"));
            tbl.Columns.Add(new DataColumn("Type"));
            tbl.Columns.Add(new DataColumn("SubType"));
            tbl.Columns.Add(new DataColumn("Library"));



            string[] lines = System.IO.File.ReadAllLines(filePath);

            foreach (string line in lines)
            {
                var cols = line.Split(',');

                DataRow row = tbl.NewRow();
                for (int i = 0; i < (int)ColumnIDs.Count; i++)
                {
                    row[i] = cols[i];
                }

                tbl.Rows.Add(row);
            }

            return tbl;
        }

        public bool ContainFactors(string F1, string F2)
        {
            bool fOk = false;

            foreach(DataRow row in Table.Rows)
            {
                string t = (string)row[(int)ColumnIDs.Type];
                string t2 = (string)row[(int)ColumnIDs.SubType];

                if ((t == F1 && t2 == F2) || (t == F2 && t2 == F1)) 
                { 
                    fOk = true; 
                    break; 
                }

            }

            return fOk; 
        }

    }
}
