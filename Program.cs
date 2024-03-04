
namespace NACHAParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputACHFile = args[0];
            string outputJSONFile = args[1];
            string outputCSVFile = args[2];

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