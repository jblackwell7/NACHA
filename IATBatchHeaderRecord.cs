
using Newtonsoft.Json;

namespace NACHAParser
{
    public class IATBatchHeaderRecord
    {
        #region Properties

        [JsonProperty("iatBchHeaderId")]
        public string IatBchHeaderId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClassCode")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("iatIndicator")]
        public string IatIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignExchangeIndicator")]
        public string ForeignExchangeIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignRefIndicator")]
        public string ForeignRefIndicator { get; set; } = string.Empty;

        [JsonProperty("foreignRef")]
        public int ForeignRef { get; set; }

        [JsonProperty("isoDestionationCountryCode")]
        public int IsoDestionationCountryCode { get; set; }

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; } = string.Empty;

        [JsonProperty("originatorId")]
        public string OriginatorId { get; set; } = string.Empty;

        [JsonProperty("SECCode")]
        public string SECCode { get; set; } = string.Empty;

        [JsonProperty("coEntryDescription")]
        public string CoEntryDescription { get; set; } = string.Empty;

        [JsonProperty("isoOriginatingCurrencyCode")]
        public string IsoOriginatingCurrencyCode { get; set; } = string.Empty;

        [JsonProperty("isoDestionationCurrencyCode")]
        public string IsoDestionationCurrencyCode { get; set; } = string.Empty;

        [JsonProperty("effectiveDate")]
        public string EffectiveDate { get; set; } = string.Empty;

        [JsonProperty("SettlementDate")]
        public string SettlementDate { get; set; } = string.Empty;

        [JsonProperty("originatorStatusCode")]
        public string OriginatorStatusCode { get; set; } = string.Empty;

        [JsonProperty("goIdOriginatingDfiId")]
        public string GoIdOriginatingDfiId { get; set; } = string.Empty;

        [JsonProperty("bchNum")]
        public string BchNum { get; set; } = string.Empty;

        #endregion
    }

}