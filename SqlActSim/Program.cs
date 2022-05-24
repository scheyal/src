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
    Console.WriteLine(" ActivityType: [power | TD]");
    Console.WriteLine(" Total: count of activities");
    Console.WriteLine(" Watermark: Stamp activity name, description. One word. Optional.");
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

if (activityType == "power")
{
    A = new Electricity();
}
else if (activityType == "TD")
{
    A = new TDActivity();
}
else
{
    usage("Error: Invalid activity type. Use only 'power' or 'TD'.");
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

if (cmdargs.Length == 4)
{
    GlobalStatic.Watermark = cmdargs[3];
}

Console.WriteLine($"Ingestion {MaxActivities} activities of type {activityType} with watermark {GlobalStatic.Watermark}");


//
// Test
//

if(GlobalStatic.Watermark == "TestOnly3Lines")
{
    Console.WriteLine("Generating 3 test lines...");
    A.GenerateTestLines(3);
    return;

}

SqlMgr DB = new SqlMgr();
DB.TestAccess();

//
// Ingest activities into SQL 
// 

for (int i = 0; i < MaxActivities; i++)
{
    GlobalStatic.Counter = i + 1;
    DB.UpdateDB(A);
    // Thread.Sleep(10);
}

