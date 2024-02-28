
using Newtonsoft.Json;

namespace NACHAParser
{
    public class IATBatchTrailer
    {
        #region Properties

        [JsonProperty("iatBchControlId")]
        public string IatBchControlId { get; set; }= string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("serviceClass")]
        public ServiceClassCode ServiceClassCode { get; set; }

        [JsonProperty("entAddendaCnt")]
        public string EntAddendaCnt { get; set; }= string.Empty;

        [JsonProperty("entHash")]
        public string EntHash { get; set; }= string.Empty;

        [JsonProperty("totBchDrEntAmt")]
        public string TotBchDrEntAmt { get; set; }= string.Empty;

        [JsonProperty("totBchCrEntAmt")]
        public string TotBchCrEntAmt { get; set; }= string.Empty;

        [JsonProperty("coId")]
        public string CoId { get; set; }= string.Empty;

        [JsonProperty("messageAuthCode")]
        public string MessageAuthCode { get; set; }= string.Empty;

        [JsonProperty("Reserved")]
        public string Reserved { get; set; }= string.Empty;

        [JsonProperty("originatingDFIId")]
        public string OriginatingDFIId { get; set; }= string.Empty;

        [JsonProperty("bchNum")]
        public string BchNum { get; set; }= string.Empty;
        
        #endregion
    }

}