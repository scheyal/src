

namespace SqlActSim
{
    /// <summary>
    /// Default Data Store as in MSM default data
    /// </summary>
    static public class DefaultDataStore
    {

        static readonly public string[] ContractualInstrumentType =
        {
            "Green Republic Cooling",
            "Green Republic Electricity Services Solar",
            "Green Republic Electricity Services Wind",
            "Green Republic Heating",
            "Utility Company Cooling",
            "Utility Company Grid Energy",
            "Utility Company Heating"
        };

        static readonly public string[] Currency =
        {
            "USD",
            "USD 2018",
            "SGD",
            "GBP",
            "CRC"
        };

        static readonly public string[] DataQualityType =
        {
            "Actual",
            "Estimated",
            "Metered"
        };
        
        static readonly public string[] DisposalMethod =
        {
            "Combusted",
            "Composted",
            "Landfilled",
            "Recycled"
        };

        static readonly public string[] Distance =
        {
            "Km",
            "mile",
            "mi",
            "ft"
        };

        static readonly public string[] Facilities =
        {
            "Contoso Factory",
            "Contoso Warehouse",
            "Contoso Pod Factory 1",
            "Contoso Pod Factory 2",
            "Contoso Pod Factory 3",
            "Contoso Pod Factory 4",
            "Contoso Pod Factory 5",
            "Contoso HQ",
            "Contoso Farms Costa Rica",
            "Contoso Farms Brazil",
            "Contoso Farms Ethiopia",
            "Contoso APAC Japan",
            "Contoso EUR HQ Bern",
            "Contoso Africa HQ Nairobi"
        };

        static readonly public string[] FuelQuantityUnit =
        {
            "L",
            "US gallon",
            "Gallon",
            "bbl",
            "litres"
        };

        static readonly public string[] FuelType =
        {
            "Anthracite",
            "Aviation Gasoline",
            "Bamboo",
            "Biodiesel (100%)",
            "Biodiesels",
            "Biogasoline",
            "Biomass Fuels - Gaseous - Landfill Gas",
            "Biomass Fuels - Gaseous - Other Biomass Gases",
            "Biomass Fuels - Liquid - Biodiesel (100%)",
            "Biomass Fuels - Liquid - Ethanol (100%)",
            "Biomass Fuels - Liquid - Rendered Animal Fat",
            "Biomass Fuels - Liquid - Vegetable Oil",
            "Biomass Fuels - Solid - Agricultural Byproducts",
            "Biomass Fuels - Solid - Peat",
            "Biomass Fuels - Solid - Solid Byproducts",
            "Biomass Fuels - Solid - Wood and Wood Residuals",
            "Bitumen",
            "Blast Furnace Gas",
            "Brown Coal Briquettes",
            "Charcoal",
            "Coal and Coke - Anthracite Coal",
            "Coal and Coke - Bituminous Coal",
            "Coal and Coke - Coal Coke",
            "Coal and Coke - Lignite Coal",
            "Coal and Coke - Mixed (Commercial Sector)",
            "Coal and Coke - Mixed (Electric Power Sector)",
            "Coal and Coke - Mixed (Industrial Coking)",
            "Coal and Coke - Mixed (Industrial Sector)",
            "Coal and Coke - Sub-bituminous Coal",
            "Coal Tar",
            "Coke Oven Coke and Lignite Coke",
            "Coke Oven Gas",
            "Coking Coal",
            "Compressed Natural Gas (CNG)",
            "Crude Oil",
            "Diesel Fuel",
            "Diesel Oil",
            "Ethane",
            "Ethanol (100%)",
            "Gas Coke",
            "Gas Oil",
            "Gas Works Gas",
            "Industrial Wastes",
            "Jet Gasoline",
            "Jet Kerosene",
            "Kerosene-Type Jet Fuel",
            "Landfill Gas",
            "Lignite",
            "Liquefied Natural Gas (LNG)",
            "Liquefied Petroleum Gases",
            "Liquefied Petroleum Gases (LPG)",
            "Lubricants",
            "Mixed (Industrial sector)",
            "Motor Gasoline",
            "Municipal Wastes (biomass fraction)",
            "Municipal Wastes (non-biomass fraction)",
            "Naphtha",
            "Natural Gas",
            "Natural Gas - Natural Gas",
            "Natural Gas Liquids(NGLs)",
            "Oil Shale and Tar Sands",
            "Orimulsion",
            "Other Biogas",
            "Other Bituminous Coal",
            "Other Fuels - Gaseous - Blast Furnace Gas",
            "Other Fuels - Gaseous - Coke Oven Gas",
            "Other Fuels - Gaseous - Fuel Gas",
            "Other Fuels - Gaseous - Propane Gas",
            "Other Fuels - Solid - Municipal Solid Waste",
            "Other Fuels - Solid - Petroleum Coke (Solid)",
            "Other Fuels - Solid - Plastics",
            "Other Fuels - Solid - Tires",
            "Other Kerosene",
            "Other Liquid Biofuels",
            "Other Petroleum Products",
            "Other Primary Solid Biomass",
            "Oxygen Steel Furnace Gas",
            "Patent Fuel",
            "Peat",
            "Petroleum Coke",
            "Petroleum Products - Asphalt and Road Oil",
            "Petroleum Products - Aviation Gasoline",
            "Petroleum Products - Butane",
            "Petroleum Products - Butylene",
            "Petroleum Products - Crude Oil",
            "Petroleum Products - Distillate Fuel Oil No. 1",
            "Petroleum Products - Distillate Fuel Oil No. 2",
            "Petroleum Products - Distillate Fuel Oil No. 4",
            "Petroleum Products - Ethane",
            "Petroleum Products - Ethylene",
            "Petroleum Products - Heavy Gas Oils",
            "Petroleum Products - Isobutane",
            "Petroleum Products - Isobutylene",
            "Petroleum Products - Kerosene",
            "Petroleum Products - Kerosene-Type Jet Fuel",
            "Petroleum Products - Liquefied Petroleum Gases (LPG)",
            "Petroleum Products - Lubricants",
            "Petroleum Products - Motor Gasoline",
            "Petroleum Products - Naphtha (<401 deg F)",
            "Petroleum Products - Natural Gasoline",
            "Petroleum Products - Other Oil (>401 deg F)",
            "Petroleum Products - Pentanes Plus",
            "Petroleum Products - Petrochemical Feedstocks",
            "Petroleum Products - Propane",
            "Petroleum Products - Propylene",
            "Petroleum Products - Residual Fuel Oil No. 5",
            "Petroleum Products - Residual Fuel Oil No. 6",
            "Petroleum Products - Special Naphtha",
            "Petroleum Products - Unfinished Oils",
            "Petroleum Products - Used Oil",
            "Plastics",
            "Refinery Feedstocks",
            "Refinery Gas",
            "Residual Fuel Oil",
            "Shale Oil",
            "Sludge Gas",
            "Sub-Bituminous Coal",
            "Sulphite Lyes (Black Liquor)",
            "Waste Oils",
            "Waxes",
            "White Spirit & SBP",
            "Wood and wood residuals",
            "Wood/Wood Waste"
        };

        static readonly public string EnergyProviderName = "Fabrikam";

        static readonly public string[] EnergyType =
        {
            "MWh",
            "kWh"
        };

        static readonly public string[] IndustrialProcessType =
        {
            "Cement Production",
            "Chemical Process",
            "Coal and Coke",
            "HVAC",
            "Natural Gas"
        };

        static readonly public string[] Material =
        {
            "Aluminum Cans",
            "Aluminum Ingot",
            "Asphalt Concrete",
            "Asphalt Shingles",
            "Beef",
            "Branches",
            "Bread",
            "Carpet",
            "Clay Bricks",
            "Concrete",
            "Copper Wire",
            "Corrugated Containers",
            "CRT Displays",
            "Dairy Products",
            "Desktop CPUs",
            "Dimensional Lumber",
            "Drywall",
            "Electronic Peripherals",
            "Fiberglass Insulation",
            "Flat-panel Displays",
            "Fly Ash",
            "Food Waste",
            "Food Waste (meat only)",
            "Food Waste (non-meat)",
            "Fruits and Vegetables",
            "Glass",
            "Grains",
            "Grass",
            "Hard-copy Devices",
            "HDPE",
            "LDPE",
            "Leaves",
            "LLDPE",
            "Magazines/Third-class mail",
            "Medium-density Fiberboard",
            "Mixed Electronics",
            "Mixed Metals",
            "Mixed MSW",
            "Mixed Organics",
            "Mixed Paper (general)",
            "Mixed Paper (primarily from offices)",
            "Mixed Paper (primarily residential)",
            "Mixed Plastics",
            "Mixed Recyclables",
            "Newspaper",
            "Office Paper",
            "PET",
            "Phonebooks",
            "PLA",
            "Portable Electronic Devices",
            "Poultry",
            "PP",
            "PS",
            "PVC",
            "Steel Cans",
            "Textbooks",
            "Tires",
            "Vinyl Flooring",
            "Wood Flooring",
            "Yard Trimmings"
        };

        static readonly public string[] OUs =
        {
            "Contoso Africa",
            "Contoso Ag",
            "Contoso APAC",
            "Contoso Corp",
            "Contoso EUR",
            "Contoso Kenya",
            "Contoso London",
            "Contoso New York",
            "Contoso Pod Business",
            "Contoso USA"
        };

        static readonly public string[] SpendType =
        {
            "Accommodation",
            "Administrative and support services",
            "Air transportation",
            "Ambulatory health care services",
            "Amusements, gambling, and recreation industries",
            "Apparel and leather and allied products",
            "Broadcasting and telecommunications",
            "Chemical products",
            "Computer and electronic products",
            "Computer systems design and related services",
            "Construction",
            "Data processing, internet publishing, and other information services",
            "Educational services",
            "Electrical equipment, appliances, and components",
            "Fabricated metal products",
            "Farms",
            "Federal Reserve banks, credit intermediation, and related activities",
            "Food and beverage and tobacco products",
            "Food and beverage stores",
            "Food services and drinking places",
            "Forestry, fishing, and related activities",
            "Funds, trusts, and other financial vehicles",
            "Furniture and related products",
            "General merchandise stores",
            "Hospitals",
            "Housing",
            "Insurance carriers and related activities",
            "Legal services",
            "Machinery",
            "Management of companies and enterprises",
            "Mining, except oil and gas",
            "Miscellaneous manufacturing",
            "Miscellaneous professional, scientific, and technical services",
            "Motion picture and sound recording industries",
            "Motor vehicle and parts dealers",
            "Motor vehicles, bodies and trailers, and parts",
            "Nonmetallic mineral products",
            "Nursing and residential care facilities",
            "Oil and gas extraction",
            "Other real estate",
            "Other retail",
            "Other services, except government",
            "Other transportation and support activities",
            "Other transportation equipment",
            "Paper products",
            "Performing arts, spectator sports, museums, and related activities",
            "Petroleum and coal products",
            "Pipeline transportation",
            "Plastics and rubber products",
            "Primary metals",
            "Printing and related support activities",
            "Publishing industries, except internet (includes software)",
            "Rail transportation",
            "Rental and leasing services and lessors of intangible assets",
            "Securities, commodity contracts, and investments",
            "Social assistance",
            "Support activities for mining",
            "Textile mills and textile product mills",
            "Transit and ground passenger transportation",
            "Truck transportation",
            "Utilities",
            "Warehousing and storage",
            "Waste management and remediation services",
            "Water transportation",
            "Wholesale trade",
            "Wood products"
        };

        static readonly public string[] TransportMode =
        {
            "Aircraft",
            "Light-Duty Truck",
            "Medium- and Heavy-Duty Truck",
            "Medium- and Heavy-Duty Truck - Shared Load",
            "Passenger Car",
            "Rail",
            "Waterborne Craft"
        };

        static readonly public string[] ValueChainPartner =
        {
            "Adatum Corporation",
            "Adventure Works Cycles",
            "First Up Consultants",
            "Bellows College",
            "Fourth Coffee",
            "Graphic Design Institute",
            "Contoso Pharmaceuticals",
            "Contoso Suites",
            "Consolidated Messenger",
            "Fabrikam, Inc.",
            "Fabrikam Residences",
            "Fincher Architects",
            "Humongous Insurance",
            "Lamna Healthcare Company",
            "Liberty's Delightful Sinful Bakery & Cafe",
            "Lucerne Publishing",
            "Margie's Travel",
            "Nod Publishers",
            "Northwind Traders",
            "Proseware, Inc.",
            "Relecloud",
            "Southridge Video",
            "Tailwind Traders",
            "Trey Research",
            "The Phone Company",
            "VanArsdel, Ltd.",
            "Wide World Importers",
            "Woodgrove Bank"
        };

        static readonly public string[] VehicleType =
        {
        "Agricultural EquipmentA-Diesel",
        "Agricultural EquipmentA-Gasoline (2 stroke)",
        "Agricultural EquipmentA-Gasoline (4 stroke)",
        "Agricultural EquipmentA-LPG",
        "Agricultural Offroad Trucks-Diesel",
        "Agricultural Offroad Trucks-Gasoline",
        "Aircraft-Aviation Gasoline",
        "Aircraft-Jet Fuel",
        "Airport Equipment-Diesel",
        "Airport Equipment-Gasoline",
        "Airport Equipment-LPG",
        "Buses-Biodiesel",
        "Buses-CNG",
        "Buses-Ethanol",
        "Buses-LNG",
        "Buses-LPG",
        "Buses-Methanol",
        "Construction/Mining EquipmentB-Diesel",
        "Construction/Mining EquipmentB-Gasoline (2 stroke)",
        "Construction/Mining EquipmentB-Gasoline (4 stroke)",
        "Construction/Mining EquipmentB-LPG",
        "Construction/Mining Offroad Trucks-Diesel",
        "Construction/Mining Offroad Trucks-Gasoline",
        "Gasoline heavy-duty vehicles <1981",
        "Gasoline heavy-duty vehicles 1982-84",
        "Gasoline heavy-duty vehicles 1985-86",
        "Gasoline heavy-duty vehicles 1987",
        "Gasoline heavy-duty vehicles 1988-1989",
        "Gasoline heavy-duty vehicles 1990-1995",
        "Gasoline heavy-duty vehicles 1996",
        "Gasoline heavy-duty vehicles 1997",
        "Gasoline heavy-duty vehicles 1998",
        "Gasoline heavy-duty vehicles 1999",
        "Gasoline heavy-duty vehicles 2000",
        "Gasoline heavy-duty vehicles 2001",
        "Gasoline heavy-duty vehicles 2002",
        "Gasoline heavy-duty vehicles 2003",
        "Gasoline heavy-duty vehicles 2004",
        "Gasoline heavy-duty vehicles 2005",
        "Gasoline heavy-duty vehicles 2006",
        "Gasoline heavy-duty vehicles 2007",
        "Gasoline heavy-duty vehicles 2008",
        "Gasoline heavy-duty vehicles 2009",
        "Gasoline heavy-duty vehicles 2010",
        "Gasoline heavy-duty vehicles 2011",
        "Gasoline heavy-duty vehicles 2012",
        "Gasoline heavy-duty vehicles 2013",
        "Gasoline heavy-duty vehicles 2014",
        "Gasoline heavy-duty vehicles 2015",
        "Gasoline heavy-duty vehicles 2016",
        "Gasoline heavy-duty vehicles 2017",
        "Gasoline heavy-duty vehicles 2018",
        "Gasoline light-duty trucks 1973-74",
        "Gasoline light-duty trucks 1975",
        "Gasoline light-duty trucks 1976",
        "Gasoline light-duty trucks 1977-78",
        "Gasoline light-duty trucks 1979-80",
        "Gasoline light-duty trucks 1981",
        "Gasoline light-duty trucks 1982",
        "Gasoline light-duty trucks 1983",
        "Gasoline light-duty trucks 1984",
        "Gasoline light-duty trucks 1985",
        "Gasoline light-duty trucks 1986",
        "Gasoline light-duty trucks 1987-93",
        "Gasoline light-duty trucks 1994",
        "Gasoline light-duty trucks 1995",
        "Gasoline light-duty trucks 1996",
        "Gasoline light-duty trucks 1997",
        "Gasoline light-duty trucks 1998",
        "Gasoline light-duty trucks 1999",
        "Gasoline light-duty trucks 2000",
        "Gasoline light-duty trucks 2001",
        "Gasoline light-duty trucks 2002",
        "Gasoline light-duty trucks 2003",
        "Gasoline light-duty trucks 2004",
        "Gasoline light-duty trucks 2005",
        "Gasoline light-duty trucks 2006",
        "Gasoline light-duty trucks 2007",
        "Gasoline light-duty trucks 2008",
        "Gasoline light-duty trucks 2009",
        "Gasoline light-duty trucks 2010",
        "Gasoline light-duty trucks 2011",
        "Gasoline light-duty trucks 2012",
        "Gasoline light-duty trucks 2013",
        "Gasoline light-duty trucks 2014",
        "Gasoline light-duty trucks 2015",
        "Gasoline light-duty trucks 2016",
        "Gasoline light-duty trucks 2017",
        "Gasoline light-duty trucks 2018",
        "Gasoline Motorcycles 1960-1995",
        "Gasoline Motorcycles 1996-2018",
        "Gasoline passenger cars 1973-74",
        "Gasoline passenger cars 1975",
        "Gasoline passenger cars 1976-77",
        "Gasoline passenger cars 1978-79",
        "Gasoline passenger cars 1980",
        "Gasoline passenger cars 1981",
        "Gasoline passenger cars 1982",
        "Gasoline passenger cars 1983",
        "Gasoline passenger cars 1984-93",
        "Gasoline passenger cars 1994",
        "Gasoline passenger cars 1995",
        "Gasoline passenger cars 1996",
        "Gasoline passenger cars 1997",
        "Gasoline passenger cars 1998",
        "Gasoline passenger cars 1999",
        "Gasoline passenger cars 2000",
        "Gasoline passenger cars 2001",
        "Gasoline passenger cars 2002",
        "Gasoline passenger cars 2003",
        "Gasoline passenger cars 2004",
        "Gasoline passenger cars 2005",
        "Gasoline passenger cars 2006",
        "Gasoline passenger cars 2007",
        "Gasoline passenger cars 2008",
        "Gasoline passenger cars 2009",
        "Gasoline passenger cars 2010",
        "Gasoline passenger cars 2011",
        "Gasoline passenger cars 2012",
        "Gasoline passenger cars 2013",
        "Gasoline passenger cars 2014",
        "Gasoline passenger cars 2015",
        "Gasoline passenger cars 2016",
        "Gasoline passenger cars 2017",
        "Gasoline passenger cars 2018",
        "Heavy-Duty Trucks-Biodiesel",
        "Heavy-Duty Trucks-CNG",
        "Heavy-Duty Trucks-Ethanol",
        "Heavy-Duty Trucks-LNG",
        "Heavy-Duty Trucks-LPG",
        "Heavy-Duty Trucks-Methanol",
        "Industrial/Commercial Equipment-Diesel",
        "Industrial/Commercial Equipment-Gasoline (2 stroke)",
        "Industrial/Commercial Equipment-Gasoline (4 stroke)",
        "Industrial/Commercial Equipment-LPG",
        "Lawn and Garden Equipment-Diesel",
        "Lawn and Garden Equipment-Gasoline (2 stroke)",
        "Lawn and Garden Equipment-Gasoline (4 stroke)",
        "Lawn and Garden Equipment-LPG",
        "Light-duty cars-Biodiesel",
        "Light-duty cars-CNG",
        "Light-duty cars-Ethanol",
        "Light-duty cars-LPG",
        "Light-duty cars-Methanol",
        "Light-Duty Trucks-Biodiesel",
        "Light-Duty Trucks-CNG",
        "Light-duty trucks-diesel 1960-1982",
        "Light-duty trucks-diesel 1983-1995",
        "Light-duty trucks-diesel 1996-2006",
        "Light-duty trucks-diesel 2007-2018",
        "Light-Duty Trucks-Ethanol",
        "Light-Duty Trucks-LNG",
        "Light-Duty Trucks-LPG",
        "Locomotives-Diesel",
        "Logging Equipment-Diesel",
        "Logging Equipment-Gasoline (2 stroke)",
        "Logging Equipment-Gasoline (4 stroke)",
        "Medium- and Heavy-Duty Vehicles-Diesel 1960-2006",
        "Medium- and Heavy-Duty Vehicles-Diesel 2007-2018",
        "Medium-duty trucks-Biodiesel",
        "Medium-duty trucks-CNG",
        "Medium-duty trucks-LNG",
        "Medium-duty trucks-LPG",
        "Passenger Cars-Diesel 1960-1982",
        "Passenger Cars-Diesel 1983-1995",
        "Passenger Cars-Diesel 1996-2006",
        "Passenger Cars-Diesel 2007-2018",
        "Railroad Equipment-Diesel",
        "Railroad Equipment-Gasoline",
        "Railroad Equipment-LPG",
        "Recreational Equipment-Diesel",
        "Recreational Equipment-Gasoline (2 stroke)",
        "Recreational Equipment-Gasoline (4 stroke)",
        "Recreational Equipment-LPG",
        "Ships and Boats-Diesel",
        "Ships and Boats-Gasoline (2 stroke)",
        "Ships and Boats-Gasoline (4 stroke)",
        "Ships and Boats-Residual Fuel Oil"
        };

        static readonly public string[] WeightUnit =
        {
            "Kg",
            "lb",
            "UK ton",
            "ton",
            "g"
        };

    }

    /// <summary>
    /// Common custom dimensions for MSM based on Coffee Example
    /// </summary>
    static public class CustomDimension
    {

        static readonly public string[] CarModel =
        {
            "Basic18",
            "Standard28",
            "Premium35",
            "PremiumSports40"
        };

        static readonly public string[] CarInterior =
        {
            "Basic",
            "Standard",
            "Leather",
            "Premium"
        };

        static readonly public string[] SoundSystem =
        {
            "Standard",
            "Premium",
            "Audiophile"
        };

        static readonly public long[] AssemblyCount =
        {
            100,
            1000
        };

    }
}


