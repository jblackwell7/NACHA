using NACHAParser;
using Newtonsoft.Json;

namespace NachaFileParser
{
    public class IATAddendaRecord
    {
        #region Properties

        [JsonProperty("iatAddenda10")]
        public IATAddenda10 IatAddenda10 { get; set; } = new IATAddenda10();
        public IATAddenda11 IatAddenda11 { get; set; } = new IATAddenda11();
        public IATAddenda12 IatAddenda12 { get; set; } = new IATAddenda12();
        public IATAddenda13 IatAddenda13 { get; set; } = new IATAddenda13();
        public IATAddenda14 IatAddenda14 { get; set; } = new IATAddenda14();
        public IATAddenda15 IatAddenda15 { get; set; } = new IATAddenda15();
        public IATAddenda16 IatAddenda16 { get; set; } = new IATAddenda16();

        #endregion
    }
    public class IATAddenda10
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("transcode")]
        public TransactionCode TransCode { get; set; }

        [JsonProperty("foreignPaymentAmount")]
        public string ForeignPaymentAmount { get; set; } = string.Empty;

        [JsonProperty("foreignTraceNumber")]
        public string ForeignTraceNumber { get; set; } = string.Empty;

        [JsonProperty("receivingName")]
        public string ReceivingName { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSeqNum")]
        public string EntryDetailSeqNum { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda11
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorName")]
        public string OriginatorName { get; set; } = string.Empty;

        [JsonProperty("originatorStreetAddress")]
        public string OriginatorStreetAddress { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda12
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorCityState")]
        public string OriginatorCityState { get; set; } = string.Empty;

        [JsonProperty("originatorCountryPostalCode")]
        public string OriginatorCountryPostalCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda13
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("originatorDfiName")]
        public string OriginatorDfiName { get; set; } = string.Empty;

        [JsonProperty("originatorIdNumQualifier")]
        public string OriginatorIdNumQualifier { get; set; } = string.Empty;

        [JsonProperty("originatorDfiId")]
        public string OriginatorDfiId { get; set; } = string.Empty;

        [JsonProperty("originatorDfiBranchCountryCode")]
        public string OriginatorDfiBranchCountryCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda14
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("ReceivingDfiName")]
        public string ReceivingDfiName { get; set; } = string.Empty;

        [JsonProperty("receiverIdNumQualifier")]
        public string ReceiverIdNumQualifier { get; set; } = string.Empty;

        [JsonProperty("receiverDfiId")]
        public string ReceiverDfiId { get; set; } = string.Empty;

        [JsonProperty("receiverDfiBranchCountryCode")]
        public string ReceiverDfiBranchCountryCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda15
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("receiverId")]
        public string ReceiverId { get; set; } = string.Empty;

        [JsonProperty("ReceivingStreetAddress")]
        public string ReceivingStreetAddress { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }
    public class IATAddenda16
    {
        #region Properties

        [JsonProperty("recordType")]
        public RecordType RecType { get; set; }

        [JsonProperty("addendaTypeCode")]
        public AddendTypeCode AddendaTypeCode { get; set; }

        [JsonProperty("receiverCityState")]
        public string ReceiverCityState { get; set; } = string.Empty;

        [JsonProperty("receiverCountryPostalCode")]
        public string ReceiverCountryPostalCode { get; set; } = string.Empty;

        [JsonProperty("reserved")]
        public string Reserved { get; set; } = string.Empty;

        [JsonProperty("entryDetailSequenceNumber")]
        public string EntryDetailSequenceNumber { get; set; } = string.Empty;

        #endregion
    }

}