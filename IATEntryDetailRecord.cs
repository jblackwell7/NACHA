
using Newtonsoft.Json;

namespace NACHAParser
{
    public class IATEntryDetailRecord
    {
        #region Properties

        [JsonProperty("iatEntDetailId")]
        public string IatEntDetailId { get; set; } = string.Empty;

        [JsonProperty("recType")]
        public RecordType RecType { get; set; }

        [JsonProperty("transcode")]
        public TransactionCode TransCode { get; set; }

        [JsonProperty("GoReceivingDFIId")]
        public string GoReceivingDFIId { get; set; } = string.Empty;

        [JsonProperty("checkDigit")]
        public string CheckDigit { get; set; } = string.Empty;

        [JsonProperty("AddendaRecordsNum")]
        public string AddendaRecordsNum { get; set; } = string.Empty;

        [JsonProperty("reserved1")]
        public string Reserved1 { get; set; } = string.Empty;

        [JsonProperty("amt")]
        public string Amt { get; set; } = string.Empty;

        [JsonProperty("ForeignDfiNum")]
        public string ForeignDfiNum { get; set; } = string.Empty;

        [JsonProperty("reserved2")]
        public string Reserved2 { get; set; } = string.Empty;

        [JsonProperty("GatewayOFACIndicator")]
        public string GatewayOFACIndicator { get; set; } = string.Empty;

        [JsonProperty("secondaryOFACIndicator")]
        public string SecondaryOFACIndicator { get; set; } = string.Empty;

        [JsonProperty("addendaRecordIndicator")]
        public AddendaRecordIndicator AddendaRecordIndicator { get; set; }

        [JsonProperty("traceNum")]
        public string TraceNum { get; set; } = string.Empty;

        [JsonProperty("IATaddendaRecords")]
        public List<IATAddendaRecord> IATAddendaRecords { get; set; } = new List<IATAddendaRecord>();

        #endregion
    }
}