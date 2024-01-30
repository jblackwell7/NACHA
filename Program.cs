

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
            FileWriter.WriteJsonFile(root, outputJSONFile);
            FileWriter.WriteCsvFile(root, outputCSVFile);

            Console.WriteLine($"Line Count: '{result.LinesProcessed}'");

            FileReader.AssociateIds(root);

            foreach (var batch in root.FileContents.AchFile.Batches)
            {
                int recCount = EntryDetails.CountEntryDetailRecords(batch);
                Console.WriteLine($"Batch ID: {batch.BchId}, Entry Detail Records: {recCount}");
            }
        }
    }
}
