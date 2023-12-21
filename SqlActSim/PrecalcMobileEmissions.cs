using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;
using System.Security.Policy;

namespace SqlActSim
{
    internal class PrecalcMobileEmissions : IActivity
    {
        const string TableName = "DemoPrecalcMobileEmissionsData";
        public string Name { get; set; }
        public string Scope;
        public string IsMarketBased;
        public string EmissionsSource;
        public string ActivityType;
        public string CalculationDate;
        public string TransactionDate;
        public string ConsumptionStartDate;
        public string ConsumptionEndDate;
        public string Facility;
        public string OrganizationalUnit;
        public string DataQualityType;
        public string OriginCorrelationID;
        public string CO2E;
        public string CO2EUnit;
        public string CarModel;
        public string CarInterior;
        public string SoundSystem;
        public string AssemblyCount;


        public PrecalcMobileEmissions()
        {

        }


        public SqlCommand CreateCommand(SqlConnection Connection)
        {
            // Create the InsertCommand.
            SqlCommand command = new SqlCommand(
                $"INSERT INTO {TableName} (Name, Scope, [Is Market Based], [Emissions Source], [Activity Type], [Calculation Date], [Transaction Date], [Consumption Start Date], [Consumption End Date], Facility, [Organizational Unit], [Data Quality Type], [Origin Correlation ID], CO2E, [CO2E Unit], [Car Model], [Car Interior], [Sound System], [Assembly Count] ) " +
                "VALUES (@Name, @Scope, @IsMarketBased, @EmissionsSource, @ActivityType, @CalculationDate, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @Facility, @OrganizationalUnit, @DataQualityType, @OriginCorrelationID, @CO2E, @CO2EUnit, @CarModel, @CarInterior, @SoundSystem, @AssemblyCount)",
                Connection);

            // Add the parameters for the InsertCommand.
            command.Parameters.Add(new SqlParameter("@Name", Name));
            command.Parameters.Add(new SqlParameter("@Scope", Scope));
            command.Parameters.Add(new SqlParameter("@IsMarketBased", IsMarketBased));
            command.Parameters.Add(new SqlParameter("@EmissionsSource", EmissionsSource));
            command.Parameters.Add(new SqlParameter("@ActivityType", ActivityType));
            command.Parameters.Add(new SqlParameter("@TransactionDate", TransactionDate));
            command.Parameters.Add(new SqlParameter("@CalculationDate", CalculationDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionStartDate", ConsumptionStartDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionEndDate", ConsumptionEndDate));
            command.Parameters.Add(new SqlParameter("@Facility", Facility));
            command.Parameters.Add(new SqlParameter("@OrganizationalUnit", OrganizationalUnit));
            command.Parameters.Add(new SqlParameter("@DataQualityType", DataQualityType));
            command.Parameters.Add(new SqlParameter("@OriginCorrelationID", OriginCorrelationID));
            command.Parameters.Add(new SqlParameter("@CO2E", CO2E));
            command.Parameters.Add(new SqlParameter("@CO2EUnit", CO2EUnit));
            command.Parameters.Add(new SqlParameter("@CarModel", CarModel));
            command.Parameters.Add(new SqlParameter("@CarInterior", CarInterior));
            command.Parameters.Add(new SqlParameter("@SoundSystem", SoundSystem));
            command.Parameters.Add(new SqlParameter("@AssemblyCount", AssemblyCount));

            return command;
        }

        public void FillLine(long index)
        {
            long timeTicks = DateTime.Now.ToFileTimeUtc();

            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            TimeSpan span = GlobalStatic.ConsumptionEndDate - GlobalStatic.ConsumptionStartDate;
            long period = span.Days / GlobalStatic.TotalActivities;
            DateTime start = GlobalStatic.ConsumptionStartDate.AddDays(index * period);
            DateTime end = start.AddDays(period);

            long q = Utilities.GetQuantity(timeTicks);

            EmissionsSource = "Mobile combustion";
            Name = String.Format($"{GlobalStatic.Watermark}: [#{index}] {EmissionsSource} at #{timeISOString}");
            IsMarketBased = Utilities.RandBool() ? "No" : "Yes";
            Scope = "Scope 1";
            ActivityType = Utilities.GetRandomString(DefaultDataStore.VehicleType);

            CO2E = q.ToString();
            CO2EUnit = "mtCO2e";
            DataQualityType = Utilities.GetRandomString(DefaultDataStore.DataQualityType);

            OrganizationalUnit = Utilities.GetRandomString(DefaultDataStore.OUs);
            Facility = Utilities.GetRandomString(DefaultDataStore.Facilities);
            CalculationDate = TransactionDate = end.AddDays(GlobalStatic.PostedTransactionDelay).ToShortDateString();
            ConsumptionStartDate = start.ToShortDateString();
            ConsumptionEndDate = end.ToShortDateString();
            OriginCorrelationID = timeString;

            CarModel = Utilities.GetRandomString(CustomDimension.CarModel);
            CarInterior = Utilities.GetRandomString(CustomDimension.CarInterior);
            SoundSystem = Utilities.GetRandomString(CustomDimension.SoundSystem);
            AssemblyCount = Utilities.GetQuantity(timeTicks, CustomDimension.AssemblyCount[1], CustomDimension.AssemblyCount[0]).ToString();

        }

        public void PrintLine()
        {
            Console.WriteLine($"N:{Name}; C:{CO2E}; SD: {ConsumptionStartDate}; ED: {ConsumptionEndDate} TD: {TransactionDate}; O: {OriginCorrelationID}; C: {CarModel}");
        }

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
