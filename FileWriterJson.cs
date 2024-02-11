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

            foreach (var batch in root.FileContents.AchFile.Batches)
            {
                DetailedBatchInfo(batch);
            }
        }
        private static void SummaryInfo(string outputFile, Root root)
        {
            Console.WriteLine("JSON file written to {0}", outputFile);
            Console.WriteLine("Number of Serialized Batches: {0}", root.FileContents.AchFile.Batches.Count);
            Console.WriteLine("Number of Serialized Entries: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecords.Count));
            Console.WriteLine("Number of Serialized Addenda: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecords.Sum(e => e.AddendaRecords.Count)));
        }
        private static void DetailedBatchInfo(Batch batch)
        {
            string serializedBatch = JsonConvert.SerializeObject(batch, Formatting.Indented);

            Console.WriteLine("Batch Number: {0}", batch.BatchHeader.BchNum);
            Console.WriteLine("Number of Entries: {0}", batch.EntryRecords.Count);
            Console.WriteLine("Number of Addenda: {0}", batch.EntryRecords.Sum(e => e.AddendaRecords.Count));
            Console.WriteLine($"Batch Json: {serializedBatch}");
            foreach (var entry in batch.EntryRecords)
            {
                DetailedEntryInfo(entry);
            }
        }
        private static void DetailedEntryInfo(EntryDetailRecord entry)
        {
            string serializedEntry = JsonConvert.SerializeObject(entry, Formatting.Indented);

            Console.WriteLine($"Entry Detail Json: {serializedEntry}");
            Console.WriteLine($"Entry Detail Json: {serializedEntry}");

            foreach (var addenda in entry.AddendaRecords)
            {
                DetailedAddendaInfo(addenda);
            }
        }
        private static void DetailedAddendaInfo(AddendaRecord addenda)
        {
            string serializedAddenda = JsonConvert.SerializeObject(addenda, Formatting.Indented);

            Console.WriteLine($"Addenda Json: {serializedAddenda}");
        }
    }
}

