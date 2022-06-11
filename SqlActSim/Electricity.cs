using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;

namespace SqlActSim
{

    internal class Electricity : IActivity
    {
        const string TableName = "MeteredElectricity";
        const int MeterCount = 10;
        const long MaxMWh = 1000000;


        // Electricity Line
        public string Name { get; set; }
        public string Description;
        public string EnergyType;
        public string Quantity;
        public string QuantityUnit;
        public string DataQualityType;
        public string EnergyProviderName;
        public string ContractualInstrumentType;
        public string IsRenewable;
        public string OrganizationalUnit;
        public string Facility;
        public string TransactionDate;
        public string ConsumptionStartDate;
        public string ConsumptionEndDate;
        public string Evidence;
        public string OriginCorrelationID;
        public string MeterNumber;


        public SqlCommand CreateCommand(SqlConnection Connection)
        {
            // Create the InsertCommand.
            SqlCommand command = new SqlCommand(
                $"INSERT INTO {TableName} (Name, Description, [Energy Type], Quantity, [Quantity Unit], [Data Quality Type], [Energy Provider Name], [Contractual Instrument Type], [Is Renewable], [Organizational Unit], Facility, [Transaction Date], [Consumption Start Date], [Consumption End Date], Evidence, [Origin Correlation ID], [Meter number]) " +
                "VALUES (@Name, @Description, @EnergyType, @Quantity, @QuantityUnit, @DataQualityType, @EnergyProviderName, @ContractualInstrumentType, @IsRenewable, @OrganizationalUnit, @Facility, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @Evidence, @OriginCorrelationID, @MeterNumber)", 
                Connection);

            // Add the parameters for the InsertCommand.
            command.Parameters.Add(new SqlParameter("@Name", Name));
            command.Parameters.Add(new SqlParameter("@Description", Description));
            command.Parameters.Add(new SqlParameter("@EnergyType", EnergyType));
            command.Parameters.Add(new SqlParameter("@Quantity", Quantity));
            command.Parameters.Add(new SqlParameter("@QuantityUnit", QuantityUnit));
            command.Parameters.Add(new SqlParameter("@DataQualityType", DataQualityType));
            command.Parameters.Add(new SqlParameter("@EnergyProviderName", EnergyProviderName));
            command.Parameters.Add(new SqlParameter("@ContractualInstrumentType", ContractualInstrumentType));
            command.Parameters.Add(new SqlParameter("@IsRenewable", IsRenewable));
            command.Parameters.Add(new SqlParameter("@OrganizationalUnit", OrganizationalUnit));
            command.Parameters.Add(new SqlParameter("@Facility", Facility));
            command.Parameters.Add(new SqlParameter("@TransactionDate", TransactionDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionStartDate", ConsumptionStartDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionEndDate", ConsumptionEndDate));
            command.Parameters.Add(new SqlParameter("@Evidence", Evidence));
            command.Parameters.Add(new SqlParameter("@OriginCorrelationID", OriginCorrelationID));
            command.Parameters.Add(new SqlParameter("@MeterNumber", MeterNumber));

            return command;
        }

        long GetQuantity(long timeTicks)
        {
            double rads = Math.PI / 180.0 * (double)(timeTicks % 180);
            return (long)((double)MaxMWh * Math.Sin(rads)) + MaxMWh / 2;
        }

        public void FillLine(long index)
        {
            long timeTicks = DateTime.Now.ToFileTimeUtc();

            long meterNumber = timeTicks  % MeterCount + 1000;
            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            long q = GetQuantity(timeTicks);

            Name = String.Format($"{GlobalStatic.Watermark}: {meterNumber} @ #{timeISOString} (power)");
            Description = String.Format($"{GlobalStatic.Watermark}: Sql hydrated @ {DateTime.Now.ToShortDateString()}");
            EnergyType = "Electricity";
            Quantity = q.ToString();
            QuantityUnit = "MWh";
            DataQualityType = "Metered";
            EnergyProviderName = "Ontario Public Energy";
            ContractualInstrumentType = "Ontario Contract - 1";
            IsRenewable = "No";
            OrganizationalUnit = "Alexandra Hospital";
            Facility = "Alexandra Hospital, Ingersoll";
            TransactionDate = DateTime.Now.ToShortDateString(); 
            ConsumptionStartDate = DateTime.Now.ToShortDateString();
            ConsumptionEndDate = DateTime.Now.ToShortDateString(); 
            Evidence = "cogito, ergo sum";
            OriginCorrelationID = timeString;
            MeterNumber = meterNumber.ToString();
        }

        public void PrintLine()
        {
            Console.WriteLine($"N:{Name}; D:{Description}; Q:{Quantity}; TD: {TransactionDate}; O: {OriginCorrelationID}; M: {MeterNumber}");
        }

        public void GenerateTestLines(int count)
        {
            for(int i=0; i<count; i++)
            {
                FillLine(i);
                PrintLine();
            }
        }


    }

}
