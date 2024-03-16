using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NACHAParser
{
    public class BatchHeaderRecord
    {
        #region Properties

        [JsonProperty("bchHeaderId")]
        public string BchHeaderId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClassCode")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("coName")]
        public string CoName { get; set; } = string.Empty;

        [JsonProperty("coDiscretionaryData")]
        public string? CoDiscretionaryData { get; set; }

        [JsonProperty("coId")]
        public string CoId { get; set; } = string.Empty;

        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("SECCode")]
        public StandardEntryClassCode SECCode { get; set; }

        [JsonProperty("coEntDescription")]
        public string? CoEntDescription { get; set; }

        [JsonProperty("coDescriptiveDate")]
        public string? CoDescriptiveDate { get; set; }

        [JsonProperty("effectiveEntDate")]
        public string EffectiveEntDate { get; set; } = string.Empty;

        [JsonProperty("settlementDate")]
        public string? SettlementDate { get; set; }

        [JsonProperty("originatorStatusCode")]
        public OriginatorStatusCode OriginatorStatusCode { get; set; }

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("bchNum")]
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