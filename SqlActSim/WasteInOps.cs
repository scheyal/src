using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SqlActSim
{
    internal class WasteInOps : IActivity
    {
        const string TableName = "DemoWasteInOpsData";
        const int MeterCount = 10;
        EmissionFactors FactorsMap;


        // Electricity Line
        public string Name { get; set; }
        public string Description;
        public string Facility;
        public string OrganizationalUnit;
        public string Evidence;

        public string Quantity;
        public string QuantityUnit;
        public string Cost;
        public string CostUnit;
        public string WasteQuantity;
        public string WasteQuantityUnit;
        public string Distance;
        public string DistanceUnit;
        public string FuelType;
        public string FuelQuantity;
        public string FuelQuantityUnit;

        public string DataQualityType;
        public string SpendType;
        public string TransportMode;
        public string DisposalMethod;
        public string Material;
        public string IndustrialProcessType;
        public string MeterNumber;
        public string ValueChainPartner;

        public string TransactionDate;
        public string ConsumptionStartDate;
        public string ConsumptionEndDate;

        public string OriginCorrelationID;

        public string CarModel;
        public string CarInterior;
        public string SoundSystem;
        public string AssemblyCount;


        public WasteInOps()
        {
           FactorsMap = new EmissionFactors();
           FactorsMap.LoadFactors();
        }

        public SqlCommand CreateCommand(SqlConnection Connection)
        {

            // Create the InsertCommand.
            SqlCommand command = new SqlCommand(
                $"INSERT INTO {TableName} (Name, Description, Facility, [Organizational Unit], Evidence, Quantity, [Quantity Unit], Cost, [Cost Unit], [Waste Quantity], [Waste Quantity Unit], Distance, [Distance Unit], [Fuel Type], [Fuel Quantity], [Fuel Quantity Unit], [Data Quality Type], [Spend Type], [Transport Mode], [Disposal Method], Material, [Industrial Process Type], [Meter Number], [Value Chain Partner], [Transaction Date], [Consumption Start Date], [Consumption End Date], [Origin Correlation ID], [Car Model], [Car Interior], [Sound System], [Assembly Count]) " +
                "VALUES (@Name, @Description, @Facility, @OrganizationalUnit, @Evidence, @Quantity, @QuantityUnit, @Cost, @CostUnit, @WasteQuantity, @WasteQuantityUnit, @Distance, @DistanceUnit, @FuelType, @FuelQuantity, @FuelQuantityUnit, @DataQualityType, @SpendType, @TransportMode, @DisposalMethod, @Material, @IndustrialProcessType, @MeterNumber, @ValueChainPartner, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @OriginCorrelationID, @CarModel, @CarInterior, @SoundSystem, @AssemblyCount)",
                Connection);

            command.Parameters.Add(new SqlParameter("@Name", Name));
            command.Parameters.Add(new SqlParameter("@Description", Description));
            command.Parameters.Add(new SqlParameter("@Facility", Facility));
            command.Parameters.Add(new SqlParameter("@OrganizationalUnit", OrganizationalUnit));
            command.Parameters.Add(new SqlParameter("@Evidence", Evidence));
            command.Parameters.Add(new SqlParameter("@Quantity", Quantity));
            command.Parameters.Add(new SqlParameter("@QuantityUnit", QuantityUnit));
            command.Parameters.Add(new SqlParameter("@Cost", Cost));
            command.Parameters.Add(new SqlParameter("@CostUnit", CostUnit));
            command.Parameters.Add(new SqlParameter("@WasteQuantity", WasteQuantity));
            command.Parameters.Add(new SqlParameter("@WasteQuantityUnit", WasteQuantityUnit));
            command.Parameters.Add(new SqlParameter("@Distance", Distance));
            command.Parameters.Add(new SqlParameter("@DistanceUnit", DistanceUnit));
            command.Parameters.Add(new SqlParameter("@FuelType", FuelType));
            command.Parameters.Add(new SqlParameter("@FuelQuantity", FuelQuantity));
            command.Parameters.Add(new SqlParameter("@FuelQuantityUnit", FuelQuantityUnit));
            command.Parameters.Add(new SqlParameter("@DataQualityType", DataQualityType));
            command.Parameters.Add(new SqlParameter("@SpendType", SpendType));
            command.Parameters.Add(new SqlParameter("@TransportMode", TransportMode));
            command.Parameters.Add(new SqlParameter("@DisposalMethod", DisposalMethod));
            command.Parameters.Add(new SqlParameter("@Material", Material));
            command.Parameters.Add(new SqlParameter("@IndustrialProcessType", IndustrialProcessType));
            command.Parameters.Add(new SqlParameter("@MeterNumber", MeterNumber));
            command.Parameters.Add(new SqlParameter("@ValueChainPartner", ValueChainPartner));
            command.Parameters.Add(new SqlParameter("@TransactionDate", TransactionDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionStartDate", ConsumptionStartDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionEndDate", ConsumptionEndDate));
            command.Parameters.Add(new SqlParameter("@OriginCorrelationID", OriginCorrelationID));
            command.Parameters.Add(new SqlParameter("@CarModel", CarModel));
            command.Parameters.Add(new SqlParameter("@CarInterior", CarInterior));
            command.Parameters.Add(new SqlParameter("@SoundSystem", SoundSystem));
            command.Parameters.Add(new SqlParameter("@AssemblyCount",AssemblyCount));

            return command;

        }

        public void FillLine(long index)
        {
            // Time seeds
            long timeTicks = DateTime.Now.ToFileTimeUtc();
            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            // Date calcs
            TimeSpan span = GlobalStatic.ConsumptionEndDate - GlobalStatic.ConsumptionStartDate;
            long period = span.Days / GlobalStatic.TotalActivities;
            DateTime start = GlobalStatic.ConsumptionStartDate.AddDays(index * period);
            DateTime end = start.AddDays(period);

            // ID+
            Name = String.Format($"{GlobalStatic.Watermark}: [#{index}] WasteInOps at #{timeISOString}");
            Description = String.Format($"[#{index}] {GlobalStatic.Watermark}: Sql hydrated @ {DateTime.Now.ToShortDateString()}");

            // Quantities with some permutations

            long q = Utilities.GetQuantity(timeTicks);
            string QUnit = Utilities.GetRandomString(DefaultDataStore.WeightUnit);

            Quantity = q.ToString();
            QuantityUnit = QUnit;

            WasteQuantity = q.ToString();
            WasteQuantityUnit = QUnit;

            q += (long)(q * Utilities.GetRandomFraction());
            Cost = q.ToString();
            CostUnit = Utilities.GetRandomString(DefaultDataStore.Currency);

            q -= (long)(q * Utilities.GetRandomFraction());
            FuelQuantity = q.ToString();
            FuelQuantityUnit = Utilities.GetRandomString(DefaultDataStore.FuelQuantityUnit);
            FuelType = Utilities.GetRandomString(DefaultDataStore.FuelType);

            q += (long)(q * Utilities.GetRandomFraction());
            Distance = q.ToString();
            DistanceUnit = Utilities.GetRandomString(DefaultDataStore.Distance);



            // Org
            OrganizationalUnit = Utilities.GetRandomString(DefaultDataStore.OUs);
            Facility = Utilities.GetRandomString(DefaultDataStore.Facilities);

            // Dates
            TransactionDate = end.AddDays(GlobalStatic.PostedTransactionDelay).ToShortDateString();
            ConsumptionStartDate = start.ToShortDateString();
            ConsumptionEndDate = end.ToShortDateString();

            // Other RD
            DataQualityType = Utilities.GetRandomString(DefaultDataStore.DataQualityType);
            Evidence = "cogito, ergo sum";
            OriginCorrelationID = timeString;
            SpendType = Utilities.GetRandomString(DefaultDataStore.SpendType);
            TransportMode = Utilities.GetRandomString(DefaultDataStore.TransportMode);
            IndustrialProcessType = Utilities.GetRandomString(DefaultDataStore.IndustrialProcessType);
            ValueChainPartner = Utilities.GetRandomString(DefaultDataStore.ValueChainPartner);

            // key emission factors
            bool fOk = false;
            do
            {
                Material = Utilities.GetRandomString(DefaultDataStore.Material);
                DisposalMethod = Utilities.GetRandomString(DefaultDataStore.DisposalMethod);

                fOk = FactorsMap.ContainFactors(Material, DisposalMethod);

            }
            while (!fOk);


            long meterNumber = timeTicks % MeterCount + 1000;
            MeterNumber = meterNumber.ToString();


            // Custom Dimensions
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
            for (int i = 0; i < count; i++)
            {
                FillLine(i);
                PrintLine();
            }
        }

        public bool TestFactors(string EF1, string EF2)
        {
            try
            {
                FactorsMap.LoadFactors();
                return FactorsMap.ContainFactors(EF1, EF2);
            }
            catch (Exception ex)
            {
                string msg = ex.Message;
                Console.WriteLine($"Exception: Factors map test failed. Error: {msg}");
                return false;
            }
        }

    }
}
