
namespace NACHAParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputACHFile = @"C:\Users\jblac\Desktop\VisualStudios\NachaParser\DataFiles\inputfiles\pop-debit.ach";
            string outputJSONFile = @"C:\Users\jblac\Desktop\VisualStudios\NachaParser\DataFiles\outputJSONFile.json";
            string outputCSVFile = @"C:\Users\jblac\Desktop\VisualStudios\NachaParser\DataFiles\outputCsvFile.csv";

            List<string> lines = new List<string>(File.ReadLines(inputACHFile));
            ParseDataResult result = FileReader.ParseData(lines);

            if (result.Root != null)
            {
                Root root = result.Root;
                JsonFileWriter.WriteJsonFile(root, outputJSONFile);
                CSVFileWriter.WriteCsvFile(root, outputCSVFile);

                Console.WriteLine($"Line Count: '{result.LinesProcessed}'");

                foreach (var batch in root.FileContents.AchFile.Batches)
                {
                    int recCount = EntryDetailRecord.CountEntryDetailRecords(batch);
                    Console.WriteLine($"Batch ID: {batch.BchId}, Entry Detail Records: {recCount}");
                }
            }
            else
            {
                Console.WriteLine("No data was parsed, or data is null");
            }
        }
    }
}