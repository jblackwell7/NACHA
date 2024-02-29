using Newtonsoft.Json;
namespace NACHAParser
{
    public class IATBatch
    {
        #region Properties

        [JsonProperty("iatBchId")]
        public string IATBchId { get; set; } = string.Empty;

        [JsonProperty("batchHeader")]
        public IATBatchHeaderRecord IATBatchHeader { get; set; } = new IATBatchHeaderRecord();

        [JsonProperty("iatEntryRecords")]
        public List<IATEntryDetailRecord> IATEntryRecords { get; set; } = new List<IATEntryDetailRecord>();

        #endregion
    }
}