using System.Globalization;

namespace Activity3PService.Models
{
    public class ActivityModel
    {
        const int MeterCount = 10;

        // Electricity Line
        public string Name { get; set; }
        public string Description { get; set; }
        public string Quantity { get; set; }
        public string QuantityUnit { get; set; }
        public string DataQualityType { get; set; }
        public string EnergyProviderName { get; set; }
        public string ContractualInstrumentType { get; set; }
        public string IsRenewable { get; set; }
        public string OrganizationalUnit { get; set; }
        public string Facility { get; set; }
        public string TransactionDate { get; set; }
        public string ConsumptionStartDate { get; set; }
        public string ConsumptionEndDate { get; set; }
        public string Evidence { get; set; }
        public string OriginCorrelationID { get; set; }


        public ActivityModel(string name, DateTime date, string facility, string OU, int counter = 0)
        {
            string c = counter > 0 ? $" #" + counter.ToString() : string.Empty;
            int R = Utilities.RandGen.Next();
            DateTime UtcNow = DateTime.UtcNow;
            string now = UtcNow.ToString("o", CultureInfo.InvariantCulture);

            Name = $"{name} for {date.ToShortDateString()} {c}";
            Description = $"{Name} at {now}";
            Quantity = Utilities.GetQuantity(UtcNow.Ticks).ToString();
            QuantityUnit = "kWh";
            DataQualityType = "Metered";
            EnergyProviderName = "The 3P Utilities Company";
            ContractualInstrumentType = "Utility Company Grid Energy";
            IsRenewable = "No";
            OrganizationalUnit = OU;
            Facility = facility;
            TransactionDate = UtcNow.ToShortDateString() ;
            ConsumptionStartDate = date.ToShortDateString();
            ConsumptionEndDate = date.AddDays(1).ToShortDateString();
            Evidence = "cogito, ergo sum";
            OriginCorrelationID = $"{now}.{R}";


        }

    }
}
