using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;


namespace SqlActSim
{
    internal class IndustrialProcess : IActivity
    {

        const string TableName = "IndustrialProcess";
        const long MaxCost = 1000000;
        DateTime BigBang = new DateTime(1966, 8, 1);

        // Industrial Process Line
        public string Name { get; set; }
        public string Description;
        public string Cost;
        public string CostUnit;
        public string DataQualityType;
        public string Facility;
        public string OrganizationalUnit;
        public string Evidence;
        public string TransactionDate;
        public string ConsumptionStartDate;
        public string ConsumptionEndDate;
        public string OriginCorrelationID;
        public string IndustrialProcessType;
        public string Quantity;
        public string QuantityUnit;

        public IndustrialProcess()
        {
        }

        public SqlCommand CreateCommand(SqlConnection Connection)
        {
            // Create the InsertCommand.
            string sqlstring =
                $"INSERT INTO {TableName} (Name, Description, Cost, [Cost unit], " +
                $"[Data quality type], Facility, [Organizational unit], Evidence, [Transaction Date], [Consumption Start Date], [Consumption End Date], [Origin Correlation ID], " +
                $"[Industrial process type], Quantity, [Quantity unit]) " +
                $"VALUES (@Name, @Description, @Cost, @CostUnit, " +
                $"@DataQualityType, @Facility, @OrganizationalUnit, @Evidence, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @OriginCorrelationID, " +
                $"@IndustrialProcessType, @Quantity, @QuantityUnit)";

            SqlCommand command = new SqlCommand(sqlstring, Connection);

            // Add the parameters for the InsertCommand.
            command.Parameters.Add(new SqlParameter("@Name", Name));
            command.Parameters.Add(new SqlParameter("@Description", Description));
            command.Parameters.Add(new SqlParameter("@Cost", Cost));
            command.Parameters.Add(new SqlParameter("@CostUnit", CostUnit));
            command.Parameters.Add(new SqlParameter("@DataQualityType", DataQualityType));
            command.Parameters.Add(new SqlParameter("@Facility", Facility));
            command.Parameters.Add(new SqlParameter("@OrganizationalUnit", OrganizationalUnit));
            command.Parameters.Add(new SqlParameter("@Evidence", Evidence));
            command.Parameters.Add(new SqlParameter("@TransactionDate", TransactionDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionStartDate", ConsumptionStartDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionEndDate", ConsumptionEndDate));
            command.Parameters.Add(new SqlParameter("@OriginCorrelationID", OriginCorrelationID));
            command.Parameters.Add(new SqlParameter("@Quantity", Quantity));
            command.Parameters.Add(new SqlParameter("@QuantityUnit", QuantityUnit));
            command.Parameters.Add(new SqlParameter("@IndustrialProcessType", IndustrialProcessType));

            return command;
        }

        public void FillLine(long index)
        {
            long timeTicks = DateTime.Now.ToFileTimeUtc();
            index = index < 0 ? 0 : index;
            int dayAdvance = (int)(index % (int.MaxValue - 1));
            DateTimeOffset startOffset = BigBang + new TimeSpan(dayAdvance, 0, 0, 0);
            DateTime startDate = startOffset.Date;
            DateTime endDate = new DateTime(startDate.Year, startDate.Month,DateTime.DaysInMonth(startDate.Year, startDate.Month));
            TimeSpan twentyDate = new TimeSpan(20,0,0,0); 
            DateTimeOffset transactDateOffset = startDate + twentyDate;
            DateTime transactDate = transactDateOffset.Date;
            

            // long meterNumber = timeTicks % MeterCount + 1000;
            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            long q = Utilities.GetQuantity(timeTicks);

            Name = String.Format($"{GlobalStatic.Watermark}: #{timeISOString} (TD)");
            Description = String.Format($"{GlobalStatic.Watermark}: Sql hydrated @ {DateTime.Now.ToShortDateString()}");
            long c = Utilities.GetQuantity(timeTicks, MaxCost);
            Cost = c.ToString();
            CostUnit = Utilities.RandBool() ? "USD" : "GBP";
            OrganizationalUnit = "Contoso Pod Business";
            Facility = "Contoso Warehouse";
            Evidence = "cogito, ergo sum";
            Quantity = q.ToString();
            QuantityUnit = Utilities.GetTriRand("metricton", "MWH", "US gallon");
            DataQualityType = Utilities.RandBool() ? "Actual" : "Estimated";
            TransactionDate = transactDate.ToShortDateString();
            ConsumptionStartDate = startDate.ToShortDateString();
            ConsumptionEndDate = endDate.ToShortDateString();
            OriginCorrelationID = timeString;
            IndustrialProcessType = Utilities.GetTriRand("Explosive forming", "Mass finishing", "Magnetic pulse welding");
        }

        public void GenerateTestLines(int count)
        {
            for (int i = 0; i < count; i++)
            {
                FillLine(i);
                PrintLine();
            }
        }

        public void PrintLine()
        {
            Console.WriteLine($"N:{Name}; D:{Description}; Q:{Quantity}; SD: {ConsumptionStartDate}; ED: {ConsumptionEndDate}; TD: {TransactionDate}; O: {OriginCorrelationID};");
        }
    }
}
