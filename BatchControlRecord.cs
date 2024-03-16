using Newtonsoft.Json;

namespace NACHAParser
{
    public class BatchControlRecord
    {
        #region Properties

        [JsonProperty("bchControlId")]
        public string BchControlId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClass")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("entAddendaCnt")]
        public string EntAddendaCnt { get; set; } = string.Empty;

        [JsonProperty("entHash")]
        public string EntHash { get; set; } = string.Empty;

        [JsonProperty("totBchDrEntAmt")]
        public string TotBchDrEntAmt { get; set; } = string.Empty;

        [JsonProperty("totBchCrEntAmt")]
        public string TotBchCrEntAmt { get; set; } = string.Empty;

        [JsonProperty("coId")]
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