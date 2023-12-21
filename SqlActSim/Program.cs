// See https://aka.ms/new-console-template for more information
using SqlActSim;
using System.Configuration;
using System.Reflection.Metadata.Ecma335;

Console.WriteLine("** Sql Emissions Activity Simulator **");


//
// Process command line arguments
//

void usage(string error = "")
{
    if(!String.IsNullOrEmpty(error))
    {
        Console.WriteLine(error);
    }
    Console.WriteLine("Error: Invalid command line.");
    Console.WriteLine("Usage: SqlActSim  ActivityType Total Watermark");
    Console.WriteLine("where:");
    Console.WriteLine(" ActivityType: [PE | ME | WO ]");
    Console.WriteLine("   \tPE = Purchased Electricity\n\tME = Mobile Combustion Precalc Emissions\n\tWO = Waste generated in Operations");
    Console.WriteLine(" Total: count of activities");
    Console.WriteLine(" Watermark: Stamp activity name, description. One word. Optional. 'TestOnly3Lines' for testing.");
}

string[] cmdargs = Environment.GetCommandLineArgs();

if (cmdargs.Length < 3 || cmdargs.Length > 4)
{
    // 0 = process name, 1 = first arg...
    usage();
    return;
}

string activityType = cmdargs[1];

// Create Activity
IActivity A;

try
{

    if (activityType == "PE")
    {
        A = new Electricity();
    }
    else if (activityType == "ME")
    {
        A = new PrecalcMobileEmissions();
    }
    else if (activityType == "WO")
    {
        A = new WasteInOps();
    }
    else
    {
        usage("Error: Invalid activity type. Use only 'PE', 'ME', or 'WO'.");
        return;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Exception: Cannot load activities. Error: {ex.Message}");
    return;
}


// Create max counter

long MaxActivities = 0;

bool bOk = long.TryParse(cmdargs[2], out MaxActivities);
if (!bOk || MaxActivities == 0)
{
    usage("Error: Invalid total. Specify max number of records. ");
    return;
}

GlobalStatic.TotalActivities = MaxActivities;

if (cmdargs.Length == 4)
{
    GlobalStatic.Watermark = cmdargs[3];
}

Console.WriteLine($"Ingestion {MaxActivities} activities of type {activityType} with watermark {GlobalStatic.Watermark}");


//
// Test
//

try
{
    EmissionFactors ef = new EmissionFactors();
    ef.LoadFactors();
    bool fExist = ef.ContainFactors("WTT - fuels", "Solid fuels");
    bool fMissing = ef.ContainFactors("asdf", "1234");
    if (!fExist || fMissing)
    {
        throw new Exception("Emission Factors is inconsistent.");
    }

}
catch (Exception ex)
{
    string msg = ex.Message;
    Console.WriteLine($"Exception: Emission Factors table test failed. Error: {msg}");
    return;
}


if (GlobalStatic.Watermark == "TestOnly3Lines")
{
    Console.WriteLine("Generating 3 test lines...");
    A.GenerateTestLines(3);
    return;

}


///
/// Action
/// 

SqlMgr DB = new SqlMgr();
DB.TestAccess();

//
// Ingest activities into SQL 
// 

for (int i = 0; i < MaxActivities; i++)
{
    GlobalStatic.Counter = i + 1;
    DB.UpdateDB(A, i);
    // Thread.Sleep(10);
}

