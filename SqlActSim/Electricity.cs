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
        const string TableName = "DemoElectricityData";
        const int MeterCount = 10;

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
        public string CarModel;
        public string CarInterior;
        public string SoundSystem;
        public string AssemblyCount;


        public SqlCommand CreateCommand(SqlConnection Connection)
        {
            // Create the InsertCommand.
            SqlCommand command = new SqlCommand(
                $"INSERT INTO {TableName} (Name, Description, [Energy Type], Quantity, [Quantity Unit], [Data Quality Type], [Energy Provider Name], [Contractual Instrument Type], [Is Renewable], [Organizational Unit], Facility, [Transaction Date], [Consumption Start Date], [Consumption End Date], Evidence, [Origin Correlation ID], [Meter number], [Car Model], [Car Interior], [Sound System], [Assembly Count]) " +
                "VALUES (@Name, @Description, @EnergyType, @Quantity, @QuantityUnit, @DataQualityType, @EnergyProviderName, @ContractualInstrumentType, @IsRenewable, @OrganizationalUnit, @Facility, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @Evidence, @OriginCorrelationID, @MeterNumber, @CarModel, @CarInterior, @SoundSystem, @AssemblyCount)", 
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
            command.Parameters.Add(new SqlParameter("@CarModel", CarModel));
            command.Parameters.Add(new SqlParameter("@CarInterior", CarInterior));
            command.Parameters.Add(new SqlParameter("@SoundSystem", SoundSystem));
            command.Parameters.Add(new SqlParameter("@AssemblyCount", AssemblyCount));

            return command;
        }


        public void FillLine(long index)
        {
            long timeTicks = DateTime.Now.ToFileTimeUtc();


            long meterNumber = timeTicks  % MeterCount + 1000;
            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            TimeSpan span = GlobalStatic.ConsumptionEndDate - GlobalStatic.ConsumptionStartDate;
            long period = span.Days / GlobalStatic.TotalActivities;
            DateTime start = GlobalStatic.ConsumptionStartDate.AddDays(index * period);
            DateTime end = start.AddDays(period);

            long q = Utilities.GetQuantity(timeTicks);

            EnergyType = "Electricity";
            Name = String.Format($"{GlobalStatic.Watermark}: [#{index}] {EnergyType} at #{timeISOString}");
            Description = String.Format($"[#{index}] {GlobalStatic.Watermark}: Sql hydrated @ {DateTime.Now.ToShortDateString()}");
            
            Quantity = q.ToString();
            QuantityUnit = Utilities.GetRandomString(DefaultDataStore.EnergyType);
            DataQualityType = Utilities.GetRandomString(DefaultDataStore.DataQualityType);
            EnergyProviderName = DefaultDataStore.EnergyProviderName;
            ContractualInstrumentType = Utilities.GetRandomString(DefaultDataStore.ContractualInstrumentType);
            IsRenewable = "No";
            OrganizationalUnit = Utilities.GetRandomString(DefaultDataStore.OUs);
            Facility = Utilities.GetRandomString(DefaultDataStore.Facilities);
            TransactionDate = end.AddDays(GlobalStatic.PostedTransactionDelay).ToShortDateString();
            ConsumptionStartDate = start.ToShortDateString();
            ConsumptionEndDate = end.ToShortDateString(); 
            Evidence = "cogito, ergo sum";
            OriginCorrelationID = timeString;
            MeterNumber = meterNumber.ToString();
            CarModel = Utilities.GetRandomString(CustomDimension.CarModel);
            CarInterior = Utilities.GetRandomString(CustomDimension.CarInterior);
            SoundSystem = Utilities.GetRandomString(CustomDimension.SoundSystem);
            AssemblyCount = Utilities.GetQuantity(timeTicks, CustomDimension.AssemblyCount[1], CustomDimension.AssemblyCount[0]).ToString();
        }

        public void PrintLine()
        {
            Console.WriteLine($"N:{Name}; Q:{Quantity}; SD: {ConsumptionStartDate}; ED: {ConsumptionEndDate} TD: {TransactionDate}; O: {OriginCorrelationID}; M: {MeterNumber}");
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
