using Newtonsoft.Json;

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

        #region Methods

        public static BatchHeaderRecord ParseBatchHeader(string line,int lineNumber,StandardEntryClassCode sec)
        {
            return new BatchHeaderRecord
            {
                RecType = (RecordType)int.Parse(line.Substring(0, 1)),
                ServiceClassCode = (ServiceClassCode)int.Parse(line.Substring(1, 3)),
                CoName = line.Substring(4, 16).Trim(),
                CoDiscretionaryData = line.Substring(20, 20).Trim(),
                CoId = line.Substring(40, 10),
                SECCode = sec,
                CoDescriptiveDate = line.Substring(63, 6).Trim(),
                EffectiveEntDate = line.Substring(71, 6),
                SettlementDate = line.Substring(75, 3).Trim(),
                OriginatorStatusCode = (OriginatorStatusCode)int.Parse(line.Substring(78, 1)),
                OriginatingDFIId = line.Substring(78, 8),
                BchNum = line.Substring(87, 7)
            };
        }
        public static StandardEntryClassCode ParseSEC(string value)
        {
            switch (value)
            {
                case "CCD":
                    return StandardEntryClassCode.CCD;
                case "PPD":
                    return StandardEntryClassCode.PPD;
                case "WEB":
                    return StandardEntryClassCode.WEB;
                case "TEL":
                    return StandardEntryClassCode.TEL;
                case "COR":
                    return StandardEntryClassCode.COR;
                case "POP":
                    return StandardEntryClassCode.POP;
                case "POS":
                    return StandardEntryClassCode.POS;
                case "BOC":
                    return StandardEntryClassCode.BOC;
                case "ARC":
                    return StandardEntryClassCode.ARC;
                case "RCK":
                    return StandardEntryClassCode.RCK;
                case "MTE":
                    return StandardEntryClassCode.MTE;
                case "SHR":
                    return StandardEntryClassCode.SHR;
                case "CTX":
                    return StandardEntryClassCode.CTX;
                case "IAT":
                    return StandardEntryClassCode.IAT;
                case "ENR":
                    return StandardEntryClassCode.ENR;
                case "TRC":
                    return StandardEntryClassCode.TRC;
                case "ADV":
                    return StandardEntryClassCode.ADV;
                case "XCK":
                    return StandardEntryClassCode.XCK;
                default:
                    throw new ArgumentException($"'{value}' is not a valid StandardEntryClassCode.");
            };
        }

        #endregion
    }
}