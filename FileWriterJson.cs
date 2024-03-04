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
            Console.WriteLine("Number of Serialized Batches: {0}", root.FileContents.AchFile.Batches.Count);
            Console.WriteLine("Number of Serialized Entries: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecord.Count));
            Console.WriteLine("Number of Serialized Addenda: {0}", root.FileContents.AchFile.Batches.Sum(b => b.EntryRecord.Sum(e => e.AddendaRecord.Count)));
        }
    }
}