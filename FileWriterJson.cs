using Newtonsoft.Json;

namespace NACHAParser
{
    public class JsonFileWriter
    {
        public static void WriteJsonFile(Root root, string outputFile)
        {
            string jString = JsonConvert.SerializeObject(root, Formatting.Indented);
            File.WriteAllText(outputFile, jString);

            SummaryInfo(outputFile, root);
        }
        private static void SummaryInfo(string outputFile, Root root)
        {
            Console.WriteLine("JSON file written to {0}", outputFile);
            Console.WriteLine("Number of Serialized Batches: {0}", root.FileContents.AchFile.Batches.Count);
            Console.WriteLine("Number of Serialized Entries: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecord.Count));
            Console.WriteLine("Number of Serialized Addenda: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecord.Sum(e => e.AddendaRecord.Count)));
        }
    }
}

