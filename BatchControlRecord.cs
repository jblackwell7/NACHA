using Newtonsoft.Json;

namespace NACHAParser
{
    public class BatchControlRecord
    {
        #region Properties

        [JsonProperty("batchControlId")]
        public string BchControlId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClassCode")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("entryAddendaCount")]
        public string EntAddendaCnt { get; set; } = string.Empty;

        [JsonProperty("entryHash")]
        public string EntHash { get; set; } = string.Empty;

        [JsonProperty("totBatchDrEntAmt")]
        public string TotBchDrEntAmt { get; set; } = string.Empty;

        [JsonProperty("totBatchCrEntAmt")]
        public string TotBchCrEntAmt { get; set; } = string.Empty;

        [JsonProperty("companyId")]
        public string CoId { get; set; } = string.Empty;

        [JsonProperty("messageAuthCode")]
        public string? MsgAuthCode { get; set; }
        public string? Reserved { get; set; }

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("bchNum")]
        public string BchNum { get; set; } = string.Empty;

        #endregion

        #region Constructors
        public BatchControlRecord()
        {
            BchControlId = Guid.NewGuid().ToString();
            Console.WriteLine($"BchControlId: '{BchControlId}'");
        }

        #endregion
    }
}