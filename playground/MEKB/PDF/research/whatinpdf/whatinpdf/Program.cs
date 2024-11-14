// See https://aka.ms/new-console-template for more information






using whatinpdf;

///
/// MAIN ENTRY
/// 
Console.WriteLine("PDF Extraction Test");

bool fOk = false;

try
{
    if (args.Length > 0)
    {
        foreach (var arg in args)
        {
            Console.WriteLine($"arg={arg}");
        }
        fOk = true;
    }
    else
    {
        Console.WriteLine("No arguments");
    }

    if (fOk)
    {
        string filename = args[0];
        Console.WriteLine($"File = {filename}");

        PdfParser pdfParser = new PdfParser();

        pdfParser.JustCallValidation(filename);

    }
}
catch(Exception e)
{
    Console.WriteLine($"Exception {e.Message}");
}
