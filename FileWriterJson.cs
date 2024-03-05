using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NACHAParser
{
    public class ShouldSerializeContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(string))
            {
                property.ShouldSerialize =
                instance =>
                {
                    var value = property.ValueProvider.GetValue(instance);
                    return value != null && !string.IsNullOrEmpty(value.ToString());
                };
            }
            return property;
        }
    }
    public class JsonFileWriter
    {
        public static void WriteJsonFile(Root root, string outputFile)
        {
            var settings = new JsonSerializerSettings
            {
                ContractResolver = new ShouldSerializeContractResolver(),
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            var json = JsonConvert.SerializeObject(root, settings);
            File.WriteAllText(outputFile, json);

            SummaryInfo(outputFile, root);
        }
        private static void SummaryInfo(string outputFile, Root root)
        {
            Console.WriteLine("JSON file written to {0}", outputFile);

            int totalBatches = root.FileContents.AchFile.Batches.Count;


            int totalEntries = 0;
            int totalAddenda = 0;
            foreach (var batch in root.FileContents.AchFile.Batches)
            {
                totalEntries += batch.EntryRecord.Count;

                foreach (var entry in batch.EntryRecord)
                {
                    totalAddenda += entry.AddendaRecord.Count;
                }
            }
            Console.WriteLine("Number of Batches: {0}", totalBatches);
            Console.WriteLine("Number of Entries: {0}", totalEntries);
            Console.WriteLine("Number of Addenda: {0}", totalAddenda);
        }
    }
}