﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Globalization;


namespace SqlActSim
{
 
    /// <summary>
    /// TBD!
    /// </summary>
    internal class TDActivity : IActivity
    {
        const string TableName = "TransportDistribution";
        const long MaxQuantity = 10000;

        // T&D Line
        public string Name { get; set; }
        public string Description;
        public string Cost;
        public string CostUnit;
        public string FuelQuantity;
        public string FuelQuantityUnit;
        public string GoodsQuantityMass;
        public string GoodsQuantityMassUnit;
        public string Distance;
        public string DistanceUnit;
        public string DataQualityType;
        public string Facility;
        public string OrganizationalUnit;
        public string TransportMode;
        public string Evidence;
        public string TransactionDate;
        public string ConsumptionStartDate;
        public string ConsumptionEndDate;
        public string OriginCorrelationID;
        public string FuelType;
        public string IndustrialProcessType;
        public string Quantity;
        public string QuantityUnit;
        public string SpendType;
        public string ValueChainPartner;

        public TDActivity()
        {
            throw new Exception("TBD: Need to be re-evaluated!");
        }

        public SqlCommand CreateCommand(SqlConnection Connection)
        {
            // Create the InsertCommand.
            string sqlstring =
                $"INSERT INTO {TableName} (Name, Description, Cost, [Cost unit], [Fuel quantity], [Fuel quantity unit], [Goods quantity (mass)], [Goods quantity (mass) unit], Distance, [Distance Unit], " +
                $"[Data quality type], Facility, [Organizational unit], [Transport mode], Evidence, [Transaction Date], [Consumption Start Date], [Consumption End Date], [Origin Correlation ID], " +
                $"[Fuel type], [Industrial process type], Quantity, [Quantity unit], [Spend type], [Value chain partner]) " +
                $"VALUES (@Name, @Description, @Cost, @CostUnit, @FuelQuantity, @FuelQuantityUnit, @GoodsQuantityMass, @GoodsQuantityMassUnit, @Distance, @DistanceUnit, " +
                $"@DataQualityType, @Facility, @OrganizationalUnit, @TransportMode, @Evidence, @TransactionDate, @ConsumptionStartDate, @ConsumptionEndDate, @OriginCorrelationID, " +
                $"@FuelType, @IndustrialProcessType, @Quantity, @QuantityUnit, @SpendType, @ValueChainPartner)";

            SqlCommand command = new SqlCommand(sqlstring, Connection); 

            // Add the parameters for the InsertCommand.
            command.Parameters.Add(new SqlParameter("@Name", Name));
            command.Parameters.Add(new SqlParameter("@Description", Description));
            command.Parameters.Add(new SqlParameter("@Cost", Cost));
            command.Parameters.Add(new SqlParameter("@CostUnit", CostUnit));
            command.Parameters.Add(new SqlParameter("@FuelQuantity", FuelQuantity));
            command.Parameters.Add(new SqlParameter("@FuelQuantityUnit", FuelQuantityUnit));
            command.Parameters.Add(new SqlParameter("@GoodsQuantityMass", GoodsQuantityMass));
            command.Parameters.Add(new SqlParameter("@GoodsQuantityMassUnit", GoodsQuantityMassUnit));
            command.Parameters.Add(new SqlParameter("@Distance", Distance));
            command.Parameters.Add(new SqlParameter("@DistanceUnit", DistanceUnit));
            command.Parameters.Add(new SqlParameter("@DataQualityType", DataQualityType));
            command.Parameters.Add(new SqlParameter("@Facility", Facility));
            command.Parameters.Add(new SqlParameter("@OrganizationalUnit", OrganizationalUnit));
            command.Parameters.Add(new SqlParameter("@TransportMode", TransportMode));
            command.Parameters.Add(new SqlParameter("@Evidence", Evidence));
            command.Parameters.Add(new SqlParameter("@TransactionDate", TransactionDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionStartDate", ConsumptionStartDate));
            command.Parameters.Add(new SqlParameter("@ConsumptionEndDate", ConsumptionEndDate));
            command.Parameters.Add(new SqlParameter("@OriginCorrelationID", OriginCorrelationID));
            command.Parameters.Add(new SqlParameter("@Quantity", Quantity));
            command.Parameters.Add(new SqlParameter("@QuantityUnit", QuantityUnit));
            command.Parameters.Add(new SqlParameter("@FuelType", FuelType));
            command.Parameters.Add(new SqlParameter("@IndustrialProcessType", IndustrialProcessType));
            command.Parameters.Add(new SqlParameter("@SpendType", SpendType));
            command.Parameters.Add(new SqlParameter("@ValueChainPartner", ValueChainPartner));

            return command;
        }

        public void FillLine(long index)
        {
            long timeTicks = DateTime.Now.ToFileTimeUtc();

            // long meterNumber = timeTicks % MeterCount + 1000;
            string timeString = timeTicks.ToString();
            string timeISOString = DateTime.Now.ToString("o", CultureInfo.GetCultureInfo("en-US"));

            long q = Utilities.GetQuantity(timeTicks);
            long d = q;  

            Name = String.Format($"{GlobalStatic.Watermark}: #{timeISOString} (TD)");
            Description = String.Format($"{GlobalStatic.Watermark}: Sql hydrated @ {DateTime.Now.ToShortDateString()}");
            long c = q / 2;
            Cost = c.ToString();
            CostUnit = Utilities.RandBool()? "USD" : "GBP";
            long f = q / 3;
            FuelQuantity = f.ToString();
            FuelQuantityUnit = Utilities.GetTriRand("L", "US gallon", "Gallon");
            FuelType = Utilities.GetTriRand("Jet Gasoline", "Shale Oil", "Liquefied Petroleum Gases (LPG)");
            GoodsQuantityMass = q.ToString();
            GoodsQuantityMassUnit = Utilities.GetTriRand("Kg", "lb","UK ton"); 
            Distance = d.ToString();
            DistanceUnit = Utilities.RandBool() ? "Km" : "mile";
            TransportMode = Utilities.GetTriRand("Aircraft", "Waterborn Craft", "Light-Duty Truck");
            OrganizationalUnit = "Contoso Africa";
            Facility = "Contoso Africa HQ Nairobi";
            Evidence = "cogito, ergo sum";
            Quantity = "0"; 
            QuantityUnit = "mile";
            DataQualityType = Utilities.RandBool() ? "Actual" : "Estimated";
            TransactionDate = DateTime.Now.ToShortDateString();
            ConsumptionStartDate = DateTime.Now.ToShortDateString();
            ConsumptionEndDate = DateTime.Now.ToShortDateString();
            OriginCorrelationID = timeString;
            IndustrialProcessType = Utilities.GetTriRand("Motor Gasoline", "Diesel Fuel", "Natural Gas");
            ValueChainPartner= Utilities.GetTriRand("Trey Research", "Southridge Video", "Bellows College");
            SpendType = Utilities.GetTriRand("Primary metals", "Computer systems design and related services", "Performing arts, spectator sports, museums, and related activities");

        }

        public void PrintLine()
        {
            Console.WriteLine($"N:{Name}; D:{Description}; GQ:{GoodsQuantityMass}; TD: {TransactionDate}; O: {OriginCorrelationID}; TM: {TransportMode}");
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
