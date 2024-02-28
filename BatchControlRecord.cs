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

        #region Methods

        public static BatchControlRecord ParseBatchControl(string line)
        {
            return new BatchControlRecord
            {
                RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                ServiceClassCode = (ServiceClassCode)int.Parse(line.Substring(1, 3)),
                EntAddendaCnt = line.Substring(4, 6),
                EntHash = line.Substring(10, 10),
                TotBchDrEntAmt = line.Substring(20, 12),
                TotBchCrEntAmt = line.Substring(32, 12),
                CoId = line.Substring(44, 10),
                MsgAuthCode = line.Substring(54, 19),
                Reserved = line.Substring(73, 6),
                OriginatingDFIId = line.Substring(79, 8),
                BchNum = line.Substring(87, 7)
            };
        }

        #endregion
    }
}