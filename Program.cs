

using NachaFileParser;

namespace NACHAParser
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputACHFile = @"";
            string outputJSONFile = @"";
            string outputCSVFile = @"";

            ParseDataResult result = FileReader.ParseData(inputACHFile);

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
    }
}
