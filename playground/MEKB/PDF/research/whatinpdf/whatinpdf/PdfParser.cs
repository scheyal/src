using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Writer;
using Tabula.Detectors;
using Tabula.Extractors;
using Tabula;


// LIB DOC: https://github.com/UglyToad/PdfPig/wiki

namespace whatinpdf
{
    public class PdfParser
    {
        public void DumpPdf(string filename)
        {
            using (PdfDocument document = PdfDocument.Open(filename, new ParsingOptions() { ClipPaths = true }))
            {
                foreach (Page page in document.GetPages())
                {
                    if(page.Number > 2)
                    {
                        break;
                    }
                    Console.WriteLine("Dump Method: DumpPdf - word dump");
                    Console.WriteLine($"\n\n ** Page Number = {page.Number} ; Width = {page.Width} ; Height = {page.Height} ; Size Type = {page.Size} **\n");

                    IEnumerable<Word> words = page.GetWords();

                    foreach (Word word in words)
                    {
                        Console.Write(word.ToString());
                        Console.Write(' ');
                    }
                    Console.WriteLine("\n\n---");

                    PrintPdfDetails(document, page.Number, filename);
                    Console.WriteLine("\n\n--- ---");

                    DumpPdfTable(document, page.Number, filename);
                    Console.WriteLine("\n\n--- --- ---");

                }
            }
        }
    
        public void PrintPdfDetails(PdfDocument document, int pageNumber, string filename = "output.pdf")
        {
            Console.WriteLine("Method: PrintPdfDetails - wordExtractor with ordered blocks");
            var outputPath = "P" + pageNumber + "-" + filename;
            var builder = new PdfDocumentBuilder { };
            PdfDocumentBuilder.AddedFont font = builder.AddStandard14Font(Standard14Font.Helvetica);
            var pageBuilder = builder.AddPage(document, pageNumber);
            pageBuilder.SetStrokeColor(0, 255, 0);
            var page = document.GetPage(pageNumber);

            var letters = page.Letters; // no preprocessing

            // 1. Extract words
            var wordExtractor = NearestNeighbourWordExtractor.Instance;

            var words = wordExtractor.GetWords(letters);

            // 2. Segment page
            var pageSegmenter = DocstrumBoundingBoxes.Instance;

            var textBlocks = pageSegmenter.GetBlocks(words);

            // 3. Postprocessing
            var readingOrder = UnsupervisedReadingOrderDetector.Instance;
            var orderedTextBlocks = readingOrder.Get(textBlocks);

            // 4. Add debug info - Bounding boxes and reading order
            foreach (var block in orderedTextBlocks)
            {
                var bbox = block.BoundingBox;
                int i = block.ReadingOrder;
                Console.WriteLine($"{i}> {block.Text}");
                pageBuilder.DrawRectangle(bbox.BottomLeft, (decimal)(bbox.Width), (decimal)(bbox.Height));
                pageBuilder.AddText($"({block.ReadingOrder.ToString()})", 8, bbox.TopLeft, font);
            }

            // 5. Write result to a file
            byte[] fileBytes = builder.Build();
            File.WriteAllBytes(outputPath, fileBytes); // save to file
        }

        public void DumpPdfTable(PdfDocument document, int pageNumber, string filename)
        {
            ObjectExtractor oe = new ObjectExtractor(document);
            PageArea page = oe.Extract(pageNumber);

            // detect canditate table zones
            SimpleNurminenDetectionAlgorithm detector = new SimpleNurminenDetectionAlgorithm();
            var regions = detector.Detect(page);

            IExtractionAlgorithm ea = new BasicExtractionAlgorithm();
            List<Table> tables = ea.Extract(page.GetArea(regions[0].BoundingBox)); // take first candidate area
            var table = tables[0];
            var rows = table.Rows;

            Console.WriteLine($"\n\n>> TableWH: {table.RowCount}, {table.ColumnCount} <<\n");
            int i = 0;
            foreach (var row in rows)
            {
                foreach (var cell in row)
                {
                    if (cell != null && cell.GetText() != String.Empty)
                    {
                        Console.WriteLine($"R{i}.Text:{cell.GetText()}");
                    }
                }
                i++;
            }
        }

        /// <summary>
        /// DumpPdfTable2 is using SpreadsheetExtractionAlgorithm which fails on our drawing!
        /// </summary>
        /// <param name="filename"></param>
        public void DumpPdfTable2(string filename)
        {
            throw new NotImplementedException();

            using (PdfDocument document = PdfDocument.Open(filename, new ParsingOptions() { ClipPaths = true }))
            {
                ObjectExtractor oe = new ObjectExtractor(document);
                PageArea page = oe.Extract(1);

                IExtractionAlgorithm ea = new SpreadsheetExtractionAlgorithm();
                List<Table> tables = ea.Extract(page);
                var table = tables[0];
                var rows = table.Rows;
            }
        }

        public void JustCallValidation(string filename)
        {
            DumpPdf(filename);
        }

    }

 
}
