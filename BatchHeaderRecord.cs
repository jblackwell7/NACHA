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

        #region Methods

        public bool ValidateBatchHeader(string nextLine)
        {
            if (IsMandatoryField())
            {
                if (IsCompanyEntryDescription())
                {
                    return true;
                }
                else
                {
                    throw new Exception("Company Entry Description is missing.");
                }
            }
            else
            {
                throw new Exception("Batch Header Record is missing required fields.");
            }
        }
        public bool IsMandatoryField()
        {
            if (Enum.IsDefined(typeof(RecordType), RecType)
            && Enum.IsDefined(typeof(ServiceClassCode), ServiceClassCode)
            && !string.IsNullOrWhiteSpace(CoName)
            && !string.IsNullOrWhiteSpace(CoId)
            && !string.IsNullOrWhiteSpace(CoEntDescription)
            && Enum.IsDefined(typeof(StandardEntryClassCode), SECCode)
            && !string.IsNullOrWhiteSpace(EffectiveEntDate)
            && Enum.IsDefined(typeof(OriginatorStatusCode), OriginatorStatusCode)
            && !string.IsNullOrWhiteSpace(OriginatingDFIId))
            {
                return true;
            }
            else
            {
                throw new Exception("Batch Header Record is missing required fields.");
            }
        }
        public bool IsCompanyEntryDescription()
        {
            if (SECCode == StandardEntryClassCode.ENR && CoEntDescription != "AUTOENROLL")
            {
                throw new Exception($"Company Entry Description must be 'AUTOENROLL' for Standard Entry Class Code '{SECCode}'.");
            }
            if (SECCode == StandardEntryClassCode.RCK && CoEntDescription != "REDEPCHECK")
            {
                throw new Exception($"Company Entry Description must be 'REDEPCHECK' for Standard Entry Class Code '{SECCode}'.");
            }
            if (SECCode == StandardEntryClassCode.XCK && CoEntDescription != "NO CHECK")
            {
                throw new Exception($"Company Entry Description must be 'NO CHECK' for Standard Entry Class Code '{SECCode}'.");
            }

            return true;
        }

        #endregion
    }
}