using Microsoft.AspNetCore.SignalR;

namespace Activity3PService.Models
{
    public class Factor
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Type { get; set; }
        public string SubType { get; set; }
        public string DocumentationReference { get; set; }
        public string CH4 { get; set; }
        public string CH4Unit { get; set; }
        public string CO2 { get; set; }
        public string CO2Unit { get; set; }
        public string N2O { get; set; }
        public string N2OUnit { get; set; }
        public string HFCs { get; set; }
        public string HFCsUnit { get; set; }
        public string NF3 { get; set; }
        public string NF3Unit { get; set; }
        public string PFCs { get; set; }
        public string PFCsUnit { get; set; }
        public string SF6 { get; set; }
        public string SF6Unit { get; set; }
        public string CO2E { get; set; }
        public string CO2EUnit { get; set; }
        public string OtherGHGs { get; set; }
        public string OtherGHGsUnit { get; set; }
        public string BiogenicCO2Factor { get; set; }
        public string BiogenicCO2FactorUnit { get; set; }
        public string IsBiofuel { get; set; }

        public Factor()
        {
            Name = string.Empty;
            Description = string.Empty;
            Unit = string.Empty;
            Type = string.Empty;
            SubType = string.Empty;
            DocumentationReference = string.Empty;
            CH4 = string.Empty;
            CH4Unit = string.Empty;
            CO2 = string.Empty;
            CO2Unit = string.Empty;
            N2O = string.Empty;
            N2OUnit = string.Empty;
            HFCs = string.Empty;
            HFCsUnit = string.Empty;
            NF3 = string.Empty;
            NF3Unit = string.Empty;
            PFCs = string.Empty;
            PFCsUnit = string.Empty;
            SF6 = string.Empty;
            SF6Unit = string.Empty;
            CO2E = string.Empty;
            CO2EUnit = string.Empty;
            OtherGHGs = string.Empty;
            OtherGHGsUnit = string.Empty;
            BiogenicCO2Factor = string.Empty;
            BiogenicCO2FactorUnit = string.Empty;
            IsBiofuel = string.Empty;
        }
    }


    public class LibraryModel
    {
        public LibraryHeaderModel Header { get; set; }  
        public List<Factor> Factors { get; set; }
        public LibraryModel() 
        { 
            Header = new ();
            Factors = new();
        }
        public LibraryModel(LibraryHeaderModel header) 
        {  
            Header = header; 
            Factors = new();
        }
    }
}

