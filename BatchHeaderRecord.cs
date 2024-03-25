using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NACHAParser
{
    public class BatchHeaderRecord
    {
        #region Properties

        [JsonProperty("batchHeaderId")]
        public string BchHeaderId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClassCode")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("companyName")]
        public string CoName { get; set; } = string.Empty;

        [JsonProperty("companyDiscretionaryData")]
        public string? CoDiscretionaryData { get; set; }

        [JsonProperty("companyId")]
        public string CoId { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("standardEntryClassCode")]
        public StandardEntryClassCode SECCode { get; set; }

        [JsonProperty("companyEntDescription")]
        public string? CoEntDescription { get; set; }

        [JsonProperty("companyDescriptiveDate")]
        public string? CoDescriptiveDate { get; set; }

        [JsonProperty("effectiveEntDate")]
        public string EffectiveEntDate { get; set; } = string.Empty;

        [JsonProperty("settlementDate")]
        public string? SettlementDate { get; set; }

        [JsonProperty("originatorStatusCode")]
        public OriginatorStatusCode OriginatorStatusCode { get; set; }

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("batchNumber")]
        public string BchNum { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public BatchHeaderRecord()
        {
            BchHeaderId = Guid.NewGuid().ToString();
            Console.WriteLine($"BchHeaderId: '{BchHeaderId}'");
        }

        #endregion
    }
}