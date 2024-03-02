using Newtonsoft.Json;

namespace NACHAParser
{
    public class Batch
    {
        #region Properties

        [JsonProperty("bchId")]
        public string BchId { get; set; } = string.Empty;

        [JsonProperty("BatchHeader")]
        public BatchHeaderRecord? BatchHeader { get; set; }

        [JsonProperty("entryDetailRecords")]
        public List<EntryDetailRecord>? EntryRecord { get; set; } = new List<EntryDetailRecord>();

        [JsonProperty("BatchControl")]
        public BatchControlRecord? BatchControl { get; set; }

        #endregion

        #region Constructors
        public Batch()
        {
            BchId = Guid.NewGuid().ToString();
            Console.WriteLine($"BchId: '{BchId}'");
        }

        #endregion
    }
}